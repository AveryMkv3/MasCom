using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Windows;
using WebSocketSharper;
using Notif = HandyControl.Controls.Growl;

namespace MasCom.Clientv2.ViewModels
{
    public class AuthenticationVM : ViewModelBase
    {
        private WebSocket authWss;
        private bool _isConnecting = false;

        public bool IsConnecting
        {
            get { return _isConnecting; }
            set { _isConnecting = value; RaisePropertyChanged(nameof(IsConnecting)); }
        }

        public AuthenticationVM()
        {
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Connexion>();

            var config = (App.Current as App).Configuration.Get<Models.Config>();

            authWss = new WebSocket(logger: logger, url: $"ws://{config.ServerDomainName}:{config.ServerBasePort}/authenticate",
                                        alwaysReconnect: false, protocols: null);

            authWss.MessageReceived.Subscribe(OnMessageReceived);

            Messenger.Default.Register<Models.AuthModel>(this, "auth_credentials", Connect);
        }
        private async void Connect(Models.AuthModel credentials)
        {
            if (string.IsNullOrWhiteSpace(credentials.ID) || string.IsNullOrWhiteSpace(credentials.Password))
                return;

            IsConnecting = true;

            AppDomain.CurrentDomain.SetData("credentials", credentials);

            authWss.SetCredentials(credentials.ID, credentials.Password, true);

            await authWss.ConnectTaskAsync();

            await Task.Delay(TimeSpan.FromSeconds(2.5));

            if (!authWss.IsAlive)
                MessageBox.Show("Une erreur est survenue lors de la connexion, veuillez réésayer", caption: "Erreur",
                    button: MessageBoxButton.OK, icon: MessageBoxImage.Warning, defaultResult: MessageBoxResult.OK);

            IsConnecting = false;
            authWss.CloseAsync();
        }

        private void OnMessageReceived(WebMessage message)
        {
            if (message.MessageType == System.Net.WebSockets.WebSocketMessageType.Text)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Lib.AuthMessage>(message.Text);

                if (result.Status == Lib.MessageStatus.Success)
                {
                    Notif.SuccessGlobal(new HandyControl.Data.GrowlInfo()
                    {
                        ConfirmStr = "Ok",
                        Message = "Connecté",
                        ShowCloseButton = true,
                        Type = HandyControl.Data.InfoType.Success
                    });

                    AppDomain.CurrentDomain.SetData("user", result.AuthenticatedUser);

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        var mw = SimpleIoc.Default.GetInstance<MainWindow>();
                        App.Current.MainWindow = mw;

                        for (int i = 0; i < App.Current.Windows.Count; i++)
                        {
                            if (App.Current.Windows[i].Name.ToLower() == "mainwin")
                            {
                                App.Current.Windows[i].Close();
                                break;
                            }
                        }

                        mw.Show();
                    });
                }

                return;
            }

            return;
        }
    }
}
