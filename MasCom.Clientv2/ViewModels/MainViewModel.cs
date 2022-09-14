using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MasCom.Lib;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Growl = HandyControl.Controls.Growl;

namespace MasCom.Clientv2.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private List<User> _users;
        private User _selectedUser;
        private string _messageText;
        private ObservableCollection<MessageComponent> _messageComponents = new ObservableCollection<MessageComponent>();
        public Lib.User LoggedUser { get; set; }


        public List<Lib.User> Users
        {
            get { return _users; }
            set { _users = value; RaisePropertyChanged(nameof(Users)); }
        }
        public string MessageText
        {
            get { return _messageText; }
            set { _messageText = value; RaisePropertyChanged(nameof(MessageText)); }
        }

        public ObservableCollection<MessageComponent> MessageComponents
        {
            get { return _messageComponents; }
            set { _messageComponents = value; RaisePropertyChanged(nameof(MessageComponents), newValue: value); }
        }
        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                RaisePropertyChanged(nameof(SelectedUser), newValue: value);
            }
        }
        public MainViewModel()
        {
            Users = Enumerable.Empty<User>().ToList();

            Messenger.Default.Register<List<Lib.User>>(this, ReceiveUsersList);
            Messenger.Default.Register<TextMessage>(this, Models.MessageWay.In, ReceiveTextMessage);
            Messenger.Default.Register<CallMessage>(this, Models.MessageWay.In, ReceiveCall);
            
            LoggedUser = (AppDomain.CurrentDomain.GetData("user") as Lib.User);
        }

        public ICommand SendTextMessageCommand => new RelayCommand<string>(SendTextMessage);
        public ICommand StartVideoCommand => new RelayCommand(MakeCall);
        public ICommand SendFileMessageCommand => new RelayCommand(SendFileMessage);
        public ICommand OpenSettingsCommand => new RelayCommand(OpenSettings);
        public ICommand DisconnectCommand => new RelayCommand(Disconnect);
        public ICommand ShowInfosCommand => new RelayCommand(ShowAppInfos);

        #region App Commands Handlers

        private void OpenSettings()
        {
            Messenger.Default.Send(new NotificationMessage(Models.AppNotifications.OpenSettings.ToString()));
        }
        private void Disconnect()
        {
            Messenger.Default.Send(new NotificationMessage(Models.AppNotifications.Disconnect.ToString()));
        }
        private void ShowAppInfos()
        {
            Messenger.Default.Send(new NotificationMessage(Models.AppNotifications.ShowAppInfos.ToString()));
        }

        #endregion

        #region Send Message
        public void SendTextMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            //Send to be delivered
            Messenger.Default.Send<Lib.TextMessage>(
                new Lib.TextMessage()
                {
                    CreationDate = DateTime.Now,
                    Message = message,
                    MessageType = Lib.MessageType.Text,
                    SenderId = LoggedUser.Id,
                    RecipientId = SelectedUser.Id
                }, Models.MessageWay.Out);

            MessageText = string.Empty;
        }
        public void SendFileMessage()
        {
            var opfd = new OpenFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Multiselect = false,
                CheckFileExists = true,
                Title = "Sélectionner un fichier à envoyer"
            };

            opfd.ShowDialog();

            string filePath = opfd.FileName;

            if (File.Exists(filePath))
            {
                var fileInfo = new FileInfo(filePath);

                FileMessageHeaders fmH = new FileMessageHeaders()
                {
                    Delivered = false,
                    Extension = fileInfo.Extension,
                    FileName = fileInfo.FullName,
                    SenderId = LoggedUser.Id,
                    RecipientId = SelectedUser.Id
                };

                //Sent to be delivered
                Messenger.Default.Send(fmH, Models.MessageWay.Out);
            }
        }
        #endregion

        #region Receive Message
        private void ReceiveTextMessage(TextMessage obj)
        {
            //Save in db
            //Show notification

            Growl.InfoGlobal($"Nouveau Message Texte de {Users.FirstOrDefault(u => u.Id == obj.SenderId).UserName}");
        }
        #endregion 

        private void ReceiveUsersList(List<Lib.User> users)
        {
            Users.Clear();
            Users = users;
            SelectedUser = Users.First();
        }
        private void MakeCall()
        {
            Messenger.Default.Send(new CallMessage()
            {
                RecipientId = SelectedUser.Id,
                SenderId = LoggedUser.Id,
                CreationDate = DateTime.Now,
                CallResponse = CallResponse.Init
            }, Models.MessageWay.Out);

            StartVideo();
        }
        private void ReceiveCall(CallMessage obj)
        {
            var CallPopup = new PopUpCallControl();
            CallPopup.Actor = Users.FirstOrDefault(u => u.Id == obj.SenderId);
            CallPopup.Show();
        }

        private void StartVideo()
        {
            try
            {
                VisioWindow vw = new VisioWindow();
                vw.CalledUser = SelectedUser;
                vw.UsersList = Users;
                vw.ShowDialog();
            }
            catch (Exception)
            {
            }
        }
    }

}
