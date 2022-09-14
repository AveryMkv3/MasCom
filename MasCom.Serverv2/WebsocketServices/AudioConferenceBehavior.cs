using WebSocketSharper;
using WebSocketSharper.Server;

namespace MasCom.Serverv2.WebsocketServices
{
    public class AudioConferenceBehavior : WebSocketBehavior
    {
        protected override void OnOpen()
        {
            base.OnOpen();
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
        }

        private bool isSessionActive(string sessionID)
        {
            if (string.IsNullOrWhiteSpace(sessionID))
                return false;

            IWebSocketSession ss;
            Sessions.TryGetSession(sessionID, out ss);
            return ss != null ? ss.ConnectionState == WebSocketState.Open : false;
        }
    }
}
