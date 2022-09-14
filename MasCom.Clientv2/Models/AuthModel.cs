namespace MasCom.Clientv2.Models
{
    public class AuthModel : GalaSoft.MvvmLight.ObservableObject
    {
        private string _Id;

        public string ID
        {
            get => _Id;
            set => Set(nameof(ID), ref _Id, value);
        }

        private string _password;

        public string Password
        {
            get => _password;
            set => Set(nameof(Password), ref _password, value);
        }

    }
}
