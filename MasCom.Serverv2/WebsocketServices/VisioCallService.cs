using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSocketSharper.Server;
using WebSocketSharper;
using logger = Serilog.Log;
using MasCom.Serverv2.WebsocketServices;

namespace MasCom.Serverv2.WebsocketServices
{
    public class VisioCallService: WebSocketBehavior
    {
        protected async override void OnOpen()
        {
            #region Update User Video Call SID
            bool updateResult = false;
            var currentUser = await this.GetCurrentUser();

            var currentUserMappings = await this.GetCurrentUserSessionMappings();

            int id = 0;
            if (currentUserMappings == null)
            {
                currentUserMappings = new Lib.UserToSessionsMapping();

                id = (await this.RegisterUserMapping()).GetValueOrDefault(0);

                if (id == 0)
                {
                    logger.Logger.Error("Fermeture express de la connexion pour cause: [Aucune Liaison de Session Possible}");
                    Close(CloseStatusCode.ProtocolError, "Impossible d'ouvrir ou lier une session, veillez réésayer");
                }

                currentUserMappings.Id = id;
            }

            currentUserMappings.UserId = currentUser.Id;
            currentUserMappings.VideoCallSid = ID;

            updateResult = await this.UpdateUserMapping(currentUserMappings);

            if (updateResult)
                logger.Logger.Information("Requête de visio-conférence... {0}", ID);
            else
                logger.Logger.Fatal("Une erreur est survenue lors de l'appel: {0}", typeof(FileMessageHeadBehavior).FullName);

            #endregion 
        }

        protected override async void OnMessage(MessageEventArgs e)
        {
            if (e.IsText)
            {
                var msg = Newtonsoft.Json.JsonConvert.DeserializeObject<Lib.CallMessage>(e.Data);
                var sessionMapping = await this.GetSpecifiedUserSessionMappings(msg.RecipientId);
                
                if(sessionMapping != null)
                {
                    logger.Information("Video Call {0} ==> {1}", ID, sessionMapping.VideoCallSid);

                    if (isSessionActive(sessionMapping.VideoCallSid))
                        Sessions.SendTo(e.Data, sessionMapping.VideoCallSid);
                }
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
