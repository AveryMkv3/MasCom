namespace MasCom.Lib
{
    public class FileMessageHeaders : MessageBase
    {
        public string Extension { get; set; }
        public string FileName { get; set; }
        public bool Delivered { get; set; } = false;
        public string DeliveryId { get; set; }
        public string FileFetchUrl { get; set; }
        public new MessageType MessageType => MessageType.Binary;
        public SupportedFileTypes FileType { get; set; } = SupportedFileTypes.Other;
    }
}
