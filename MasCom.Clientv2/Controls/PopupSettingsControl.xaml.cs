using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace MasCom.Clientv2
{
    /// <summary>
    /// Interaction logic for PopupSettingsControl.xaml
    /// </summary>
    public partial class PopupSettingsControl : Popup
    {
        public PopupSettingsControl()
        {
            InitializeComponent();

            PlacementTarget = App.Current.MainWindow;
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;
            Placement = PlacementMode.Center;
            AllowsTransparency = true;
            StaysOpen = true;

            Messenger.Default.Register<NotificationMessage>(this, true, ExecuteNotificationOrder);
        }

        public bool Show()
        {
            IsOpen = true; return IsOpen;
        }

        public void Close()
        {
            IsOpen = false;
        }

        private void ExecuteNotificationOrder(NotificationMessage obj)
        {
            if (obj.Notification == Models.AppNotifications.CloseSettings.ToString())
            {
                Close();
            }

            if (obj.Notification == Models.AppNotifications.SaveSettings.ToString())
            {
                HandyControl.Controls.Growl.SuccessGlobal("Les réglages ont été enregistré avec succès");
                Close();
            }
        }
    }
}
