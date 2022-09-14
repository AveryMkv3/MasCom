using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace MasCom.Clientv2
{
    public partial class Connexion : Window
    {
        public Connexion()
        {
            InitializeComponent();
            ConnectBtn.Click += ConnectBtn_Click;
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            Models.AuthModel credentials = new Models.AuthModel()
            {
                ID = IDField.Text,
                Password = PasswordField.Password
            };

            Messenger.Default.Send(credentials, "auth_credentials");
        }
    }
}
