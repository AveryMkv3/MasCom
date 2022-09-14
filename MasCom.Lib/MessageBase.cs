using System;

namespace MasCom.Lib
{
    public class MessageBase : IMessage
    {
        public int Id { get; set; }
        public int RecipientId { get; set; }
        public int SenderId { get; set; }
        public MessageType MessageType { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
