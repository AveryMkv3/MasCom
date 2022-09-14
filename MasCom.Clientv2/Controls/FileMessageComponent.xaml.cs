using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MasCom.Clientv2
{
    /// <summary>
    /// Interaction logic for FileMessageComponent.xaml
    /// </summary>
    public partial class FileMessageComponent : UserControl
    {
        public FileMessageComponent()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string FileExtension { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string SavedFilePath { get; set; }

        public MessageRole MessageRole { get; set; } = MessageRole.Receiver;
        public HorizontalAlignment MessageRoleAlignment
        {
            get
            {
                if (MessageRole == MessageRole.Sender)
                    return HorizontalAlignment.Left;
                if (MessageRole == MessageRole.Receiver)
                    return HorizontalAlignment.Right;
                else
                    return HorizontalAlignment.Left;
            }
        }

        public SolidColorBrush FileExtensionBrush { get; set; } = new SolidColorBrush(Colors.Black);

        public DateTime DeliveryDate => DateTime.Now;
        public bool Viewed { get; set; } = false;
    }
}
