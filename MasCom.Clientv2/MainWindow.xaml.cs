using GalaSoft.MvvmLight.Messaging;
using MasCom.Lib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WebSocketSharper;
using Newton = Newtonsoft.Json.JsonConvert;

namespace MasCom.Clientv2
{
    public partial class MainWindow : Window
    {
        public WebSocket p2pClient { get; set; }
        public WebSocket searchClient { get; set; }
        public WebSocket fileHeadersClient { get; set; }
        public WebSocket fileDeliveryClient { get; set; }
        public WebSocket fileReceivingClient { get; set; }
        public RestClient MasComRestClient { get; set; }
        public WebSocket CallClient { get; set; }

        public List<FileMessageHeaders> FileHeadersRetrievalList { get; set; }

        private readonly Models.Config config;
        private readonly User loggedUser;
        private readonly ILogger logger;

        public MainWindow()
        {
            InitializeComponent();

            logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<MainWindow>();
            config = (App.Current as App).Configuration.Get<Models.Config>();
            loggedUser = (AppDomain.CurrentDomain.GetData("user") as User);
            Models.AuthModel credentials = (AppDomain.CurrentDomain.GetData("credentials") as Models.AuthModel);


            //Register Messengers for Messages that are going Out
            Messenger.Default.Register<TextMessage>(this, Models.MessageWay.Out, DeliverTextMessage);
            Messenger.Default.Register<FileMessageHeaders>(this, Models.MessageWay.Out, DeliverFileHeaders);
            Messenger.Default.Register<NotificationMessage>(this, ExecuteNotificationOrder);
            Messenger.Default.Register<CallMessage>(this, Models.MessageWay.Out, CallOutUser);

            //--

            MasComRestClient = new RestClient($"http://{config.ServerDomainName}:{config.ServerBasePort}");
            MasComRestClient.Authenticator = new HttpBasicAuthenticator(credentials.ID, credentials.Password);

            searchClient = new WebSocket(logger, $"ws://{config.ServerDomainName}:{config.ServerBasePort}/search", true);
            p2pClient = new WebSocket(logger, $"ws://{config.ServerDomainName}:{config.ServerBasePort}/message_user", true);
            fileReceivingClient = new WebSocket(logger, $"ws://{config.ServerDomainName}:{config.ServerBasePort}/deliver_file", true);
            fileHeadersClient = new WebSocket(logger, $"ws://{config.ServerDomainName}:{config.ServerBasePort}/message_file_headers", true);
            CallClient = new WebSocket(logger, $"ws://{config.ServerDomainName}:{config.ServerBasePort}/call_user", true);

            p2pClient.SetCredentials(credentials.ID, credentials.Password, true);
            searchClient.SetCredentials(credentials.ID, credentials.Password, true);
            fileHeadersClient.SetCredentials(credentials.ID, credentials.Password, true);
            fileReceivingClient.SetCredentials(credentials.ID, credentials.Password, true);
            CallClient.SetCredentials(credentials.ID, credentials.Password, true);

            searchClient.OnMessage += SearchClient_OnMessage;
            p2pClient.MessageReceived.Subscribe(Receive_P2P_Message);
            fileReceivingClient.OnMessage += FileReceivingClient_OnMessage;
            fileHeadersClient.OnMessage += OnFileDeliveryInfosReceived_DeliverFileOut;
            CallClient.OnMessage += OnCallorNotifReceived;

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void OnCallorNotifReceived(object sender, MessageEventArgs e)
        {
            if (e.IsText)
            {
                var msg = Newton.DeserializeObject<Lib.CallMessage>(e.Data);

                Messenger.Default.Send(msg, Models.MessageWay.In);
            }
        }

        private void CallOutUser(CallMessage obj)
        {
            var msg = Newton.SerializeObject(obj);
            CallClient.Send(msg);
        }

        private void ExecuteNotificationOrder(NotificationMessage obj)
        {
            if (obj.Notification == Models.AppNotifications.OpenSettings.ToString())
            {
                var settings = new PopupSettingsControl();
                settings.Show();
            }
        }

        private void AcceptCall(User obj)
        {
            var msg = Newton.SerializeObject(new CallMessage()
            {
                SenderId = loggedUser.Id,
                RecipientId = obj.Id,
                CallResponse = CallResponse.Accepted
            });

            CallClient.Send(msg);

            Messenger.Default.Send(new NotificationMessage(Models.AppNotifications.StartVisio.ToString()));
        }

        private async void FileReceivingClient_OnMessage(object sender, MessageEventArgs e)
        {
            var fileMsg = Newton.DeserializeObject<FileMessageHeaders>(e.Data);
            if (fileMsg != null)
            {
                //Write Message to viewport
                Dispatcher.Invoke(() =>
                {
                    WriteMessageToViewPort(new FileMessageComponent()
                    {
                        FileExtension = fileMsg.Extension,
                        FileName = Path.GetFileNameWithoutExtension(fileMsg.FileName),
                        SavedFilePath = Path.GetTempFileName(),
                        MessageRole = MessageRole.Sender,
                    });
                });

                var request = new RestRequest(fileMsg.FileFetchUrl, Method.Get);
                var fs = await MasComRestClient.DownloadStreamAsync(request);

                if (fs != null)
                {
                    var filePath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                                "MasCom",
                                                "Fichiers",
                                                $"{fileMsg.FileName}{fileMsg.Extension}");

                    using (FileStream destinationFileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        fs.CopyTo(destinationFileStream, 8);
                        destinationFileStream.Flush(true);
                        destinationFileStream.Close();
                    }

                    fs?.Dispose();
                }
                else
                    MessageBox.Show("Le Téléchargement du fichier à échoué");
            }
        }

        #region Deliver Message In
        private void Receive_P2P_Message(WebMessage e)
        {
            if (e.MessageType == System.Net.WebSockets.WebSocketMessageType.Text)
            {
                var textMsg = Newton.DeserializeObject<Lib.TextMessage>(e.Text);
                Dispatcher.Invoke(() =>
                {
                    WriteMessageToViewPort(new MessageComponent()
                    {
                        MessageText = textMsg.Message,
                        MessageRole = MessageRole.Sender
                    });
                });

                Messenger.Default.Send(textMsg, Models.MessageWay.In);
            }
        }
        private void SearchClient_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsText)
            {
                var users = Newton.DeserializeObject<List<Lib.User>>(e.Data);
                Messenger.Default.Send<List<Lib.User>>(users);
            }
        }

        #endregion

        #region Deliver Message Out
        private void DeliverFileHeaders(FileMessageHeaders fmH)
        {
            string data = Newton.SerializeObject(fmH);
            fileHeadersClient.Send(data);
        }
        private void OnFileDeliveryInfosReceived_DeliverFileOut(object sender, MessageEventArgs e)
        {
            if (e.IsText)
            {
                var fdI = Newton.DeserializeObject<FileDeliveryInfo>(e.Data);

                if (!File.Exists(fdI?.FilePath))
                    return;

                FileStream fs = null;
                var credentials = (AppDomain.CurrentDomain.GetData("credentials") as Models.AuthModel);

                fileDeliveryClient = new WebSocket(logger,
                            $"ws://{config.ServerDomainName}:{config.ServerBasePort}/deliver_file?delivery_id={fdI.DeliveryId}", true);

                fileDeliveryClient.SetCredentials(credentials.ID, credentials.Password, true);

                fileDeliveryClient.Connect();

                if (fileDeliveryClient.IsAlive)
                {
                    using (fs = new FileStream(fdI.FilePath, FileMode.Open, FileAccess.Read))
                    {
                        fileDeliveryClient.Send(fs, (int)fs.Length);
                    }
                }

                //Write Message to viewport
                Dispatcher.Invoke(() =>
                {
                    WriteMessageToViewPort(new FileMessageComponent()
                    {
                        FileExtension = Path.GetExtension(fdI.FilePath),
                        FileName = Path.GetFileNameWithoutExtension(fdI.FilePath),
                        SavedFilePath = fdI.FilePath,
                        MessageRole = MessageRole.Receiver,
                    });
                });

                fs?.Dispose();
            }
        }
        private async void DeliverTextMessage(TextMessage msg)
        {
            var strMsg = Newton.SerializeObject(msg);

            if (!p2pClient.IsAlive)
                await p2pClient.ConnectTaskAsync();

            p2pClient.Send(strMsg);

            //Send to ViewPort
            var msgComp = new MessageComponent()
            {
                MessageText = msg.Message,
                MessageRole = MessageRole.Receiver
            };

            WriteMessageToViewPort(msgComp);
        }
        #endregion

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            p2pClient?.Close(CloseStatusCode.Normal);
            searchClient?.Close(CloseStatusCode.Normal);
            fileDeliveryClient?.Close(CloseStatusCode.Normal);
            fileReceivingClient?.Close(CloseStatusCode.Normal);
            fileHeadersClient?.Close(CloseStatusCode.Normal);
            CallClient?.Close(CloseStatusCode.Normal);
        }
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            p2pClient.Connect();
            fileHeadersClient.Connect();
            fileReceivingClient.Connect();
            searchClient.Connect();
            CallClient.Connect();

            //Send Message to update users list
            await Task.Delay(TimeSpan.FromSeconds(1));

            if (searchClient.IsAlive)
                searchClient.Send("search");
        }
        private void WriteMessageToViewPort(UserControl message)
        {
            Dispatcher.Invoke(() =>
            {
                viewPort.Children.Add(message);
                AutoScrollVieport();
            });
        }
        private void AutoScrollVieport()
        {
            viewPortScroller.ScrollToEnd();
        }
    }

}
