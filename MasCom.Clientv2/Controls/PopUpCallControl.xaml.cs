using System.Windows;
using System.Windows.Controls.Primitives;
using GalaSoft.MvvmLight.Messaging;

namespace MasCom.Clientv2
{
    /// <summary>
    /// Interaction logic for PopUpCallControl.xaml
    /// </summary>
    public partial class PopUpCallControl : Popup
    {
        public Lib.User Actor { get; set; }
        public PopUpCallControl()
        {
            InitializeComponent();

            PlacementTarget = App.Current.MainWindow;
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;
            Placement = PlacementMode.Center;
            AllowsTransparency = true;
            StaysOpen = true;

            DataContext = this;
        }

        public bool Show()
        {
            IsOpen = true; return IsOpen;
        }

        public void Close()
        {
            IsOpen = false;
        }

        private void StopCallBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AcceptCall_Click(object sender, RoutedEventArgs e)
        {
            var vw = new VisioWindow();
            this.Close();
            vw.ShowDialog();
        }
    }
}
