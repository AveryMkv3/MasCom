namespace MasCom.Lib
{
    public enum MessageStatus
    {
        Error, Success
    }
    public class AuthMessage
    {
        public MessageStatus Status { get; set; }
        public string Msg { get; set; }

        public User AuthenticatedUser { get; set; }
    }
}
