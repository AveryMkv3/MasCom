using System;
using System.Windows;
using System.Windows.Controls;

namespace MasCom.Clientv2
{
    /// <summary>
    /// Interaction logic for MessageComponent.xaml
    /// </summary>
    public partial class MessageComponent : UserControl
    {
        public MessageComponent()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string MessageText { get; set; } = String.Empty;
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
        public DateTime DeliveryDate => DateTime.Now;
        public bool Viewed { get; set; } = false;
    }
}
