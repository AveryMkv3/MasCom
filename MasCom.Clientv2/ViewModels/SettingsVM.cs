using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;
using System;
using System.Windows;

namespace MasCom.Clientv2.ViewModels
{
    public class SettingsVM : ViewModelBase
    {
        public ICommand CloseSettingsCommand => new RelayCommand(CloseSettings);
        public Lib.User CurrentUser => AppDomain.CurrentDomain.GetData("user") as Lib.User;
        private void CloseSettings()
        {
            Messenger.Default.Send(new NotificationMessage(Models.AppNotifications.CloseSettings.ToString()));
        }

        public ICommand SaveSettingsCommand => new RelayCommand(SaveSettings);

        private void SaveSettings()
        {
            Messenger.Default.Send(new NotificationMessage(Models.AppNotifications.SaveSettings.ToString()));
        }

        public SettingsVM()
        {

        }
    }
}
