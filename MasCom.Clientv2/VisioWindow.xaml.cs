using AForge.Video;
using AForge.Video.DirectShow;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using WebSocketSharper;
using System.Threading.Tasks;
using Image = System.Windows.Controls.Image;


namespace MasCom.Clientv2
{
    public partial class VisioWindow : Window
    {
        FilterInfoCollection videoDevicesList;
        VideoCaptureDevice videoSource;
        public WebSocket VisioClient { get; set; }
        public Lib.User CalledUser { get; set; }
        public List<Lib.User> UsersList { get; set; }
        public Dictionary<int, string> ConferencingUsersToViewportMappings { get; set; }
        public Dictionary<string, bool> FrameToUserMappings { get; set; }
        public bool IsCaller { get; set; } = false;
        private Lib.User currentUser => AppDomain.CurrentDomain.GetData("user") as Lib.User;

        public VisioWindow()
        {
            videoDevicesList = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            videoSource = videoDevicesList.Count > 1 ? new VideoCaptureDevice(videoDevicesList[1].MonikerString) : new VideoCaptureDevice(videoDevicesList[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);

            ConferencingUsersToViewportMappings = new Dictionary<int, string>();

            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<MainWindow>();
            var config = (App.Current as App).Configuration.Get<Models.Config>();

            VisioClient = new WebSocket(logger, $"ws://{config.ServerDomainName}:{config.ServerBasePort}/videoconf", true);

            Models.AuthModel credentials = (AppDomain.CurrentDomain.GetData("credentials") as Models.AuthModel);
            VisioClient.SetCredentials(credentials.ID, credentials.Password, true);

            Messenger.Default.Register<NotificationMessage>(this, ExecuteNotificationOrder);


            InitializeComponent();

            //Init dict with default values
            FrameToUserMappings = new Dictionary<string, bool>();

            var r = Enumerable.Range(1, 11).Select((i) => new FrameToUserMapping(frameName: "img" + i)).ToList();
            r.ForEach(m => FrameToUserMappings[m.FrameName] = false);

            Closing += VisioWindow_Closing;
            Loaded += VisioWindow_Loaded;
            VisioClient.OnMessage += VisioClient_OnMessage;

            DataContext = this;
        }

        private async void VisioClient_OnMessage(object sender, MessageEventArgs e)
        {
            var decodedMessage = DecodeIncommingMessage(e.RawData);

            var username = UsersList.FirstOrDefault(u => u.Id == decodedMessage.Item1).UserName;

            var mms = new MemoryStream(decodedMessage.Item2);
            BitmapImage bmi = new BitmapImage();
            bmi.BeginInit();

            bmi.StreamSource = mms;
            bmi.CacheOption = BitmapCacheOption.OnDemand;
            bmi.EndInit();

            bmi.Freeze();


            if (!ConferencingUsersToViewportMappings.ContainsKey(decodedMessage.Item1))
            {
                var mapping = FrameToUserMappings.First(f => f.Value == false);
                ConferencingUsersToViewportMappings[decodedMessage.Item1] = mapping.Key;
                FrameToUserMappings[mapping.Key] = true;
            }

            RedirectToRequiredImageSource(decodedMessage.Item1);
            await Dispatcher.BeginInvoke(new ThreadStart(delegate { RedirectToRequiredImageSource(decodedMessage.Item1).Source = bmi; }));
        }

        private void VisioWindow_Loaded(object sender, RoutedEventArgs e)
        {
            VisioClient.Connect();
            videoSource.Start();
        }

        private void VisioWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            VisioClient.Close(CloseStatusCode.Normal);
            videoSource.SignalToStop();
        }
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                BitmapImage bitmapImage;
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        bitmap.Save(memory, ImageFormat.Jpeg);
                        memory.Position = 0;

                        var encodedMsg = EncodeOutGoingMessage(memory.ToArray(), currentUser.Id);
                        VisioClient.SendAsync(encodedMsg, null);

                        bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memory;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                    }
                }

                bitmapImage.Freeze();

                Dispatcher.BeginInvoke(new ThreadStart(delegate { LocalCam.Source = bitmapImage; }));
            }
            catch (Exception)
            {
            }
        }

        public void ExecuteNotificationOrder(NotificationMessage msg)
        {
            if (msg.Notification == Models.AppNotifications.EndVisio.ToString())
                Close();
        }

        //Sending user id have been encompassed between [ and ] special characters
        private ValueTuple<int, byte[]> DecodeIncommingMessage(byte[] msg)
        {
            var endFlag = Encoding.ASCII.GetBytes("]")[0];
            var endFlagPosition = msg.ToList().IndexOf(endFlag);
            var senderId = msg.SubArray(1, endFlagPosition - 1);
            int userId = Convert.ToInt32(Encoding.ASCII.GetString(senderId));
            byte[] payloadBuffer = msg.Skip(endFlagPosition + 1).ToArray();

            return (userId, payloadBuffer);
        }
        private byte[] EncodeOutGoingMessage(byte[] msg, int userId)
        {
            string prefixData = $"[{userId}]";
            byte[] encodedPrefix = Encoding.ASCII.GetBytes(prefixData);
            return encodedPrefix.Concat(msg).ToArray();
        }

        private Image RedirectToRequiredImageSource(int id)
        {
            if (ConferencingUsersToViewportMappings[id] == "img1")
                return img1;
            if (ConferencingUsersToViewportMappings[id] == "img2")
                return img2;
            if (ConferencingUsersToViewportMappings[id] == "img3")
                return img3;
            if (ConferencingUsersToViewportMappings[id] == "img4")
                return img4;
            if (ConferencingUsersToViewportMappings[id] == "img5")
                return img5;
            if (ConferencingUsersToViewportMappings[id] == "img6")
                return img6;
            if (ConferencingUsersToViewportMappings[id] == "img7")
                return img7;
            if (ConferencingUsersToViewportMappings[id] == "img8")
                return img8;
            if (ConferencingUsersToViewportMappings[id] == "img9")
                return img9;
            if (ConferencingUsersToViewportMappings[id] == "img10")
                return img10;
            else
                return null;
        }

        private async void PortMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((sender as Image).Name == "img1")
            {
                var bd = new Binding("Source") { Source = img1 };
                DistantCam.SetBinding(Image.SourceProperty, bd);
            }
            if ((sender as Image).Name == "img2")
            {
                var bd = new Binding("Source") { Source = img2 };
                DistantCam.SetBinding(Image.SourceProperty, bd);
            }
            if ((sender as Image).Name == "img3")
            {
                var bd = new Binding("Source") { Source = img3 };
                DistantCam.SetBinding(Image.SourceProperty, bd);
            }
            if ((sender as Image).Name == "img4")
            {
                var bd = new Binding("Source") { Source = img4 };
                DistantCam.SetBinding(Image.SourceProperty, bd);
            }
            if ((sender as Image).Name == "img5")
            {
                var bd = new Binding("Source") { Source = img5 };
                DistantCam.SetBinding(Image.SourceProperty, bd);
            }
            if ((sender as Image).Name == "img6")
            {
                var bd = new Binding("Source") { Source = img6 };
                DistantCam.SetBinding(Image.SourceProperty, bd);
            }
            if ((sender as Image).Name == "img7")
            {
                var bd = new Binding("Source") { Source = img7 };
                DistantCam.SetBinding(Image.SourceProperty, bd);
            }
            if ((sender as Image).Name == "img8")
            {
                var bd = new Binding("Source") { Source = img8 };
                DistantCam.SetBinding(Image.SourceProperty, bd);
            }
            if ((sender as Image).Name == "img9")
            {
                var bd = new Binding("Source") { Source = img9 };
                DistantCam.SetBinding(Image.SourceProperty, bd);
            }
            if ((sender as Image).Name == "img10")
            {
                var bd = new Binding("Source") { Source = img10 };
                DistantCam.SetBinding(Image.SourceProperty, bd);
            }

            await Task.CompletedTask;
        }
    }

    public class FrameToUserMapping
    {
        public FrameToUserMapping()
        {
        }

        public FrameToUserMapping(string frameName, bool isMapped = false)
        {
            FrameName = frameName;
            IsMapped = isMapped;
        }
        public bool IsMapped { get; set; }
        public string FrameName { get; set; }
    }
}
