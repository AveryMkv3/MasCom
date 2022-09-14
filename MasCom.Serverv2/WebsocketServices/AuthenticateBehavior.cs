using MasCom.Lib;
using logger = Serilog.Log;
using Newtonsoft.Json;
using WebSocketSharper;
using WebSocketSharper.Server;

namespace MasCom.Serverv2.WebsocketServices
{
    class AuthenticateBehavior : WebSocketBehavior
    {
        protected async override void OnOpen()
        {
            if (this.Context.User.Identity.IsAuthenticated && this.Context.IsAuthenticated)
            {
                var Msg = new AuthMessage()
                {
                    Msg = "Connecté avec succes",
                    Status = MessageStatus.Success,
                };

                Msg.AuthenticatedUser = await this.GetCurrentUser();

                if (isSessionActive(ID))
                {
                    Send(JsonConvert.SerializeObject(Msg));
                    logger.Logger.Information("Authentification Réussie");
                }
                else
                    CloseAsync();
            }
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
