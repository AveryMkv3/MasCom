using System;

namespace MasCom.Lib
{
    public interface IMessage
    {
        public int Id { get; set; }
        public int RecipientId { get; set; }
        public int SenderId { get; set; }
        public MessageType MessageType { get; set; }
        public DateTime CreationDate { get; set; }
    }

    public enum MessageType
    {
        Text,
        Binary,
        Call
    }
}
