using WebSocketSharper;
using WebSocketSharper.Server;
using logger = Serilog.Log;
using Newton = Newtonsoft.Json.JsonConvert;

namespace MasCom.Serverv2.WebsocketServices
{
    public class UserMessagingServiceBehavior : WebSocketBehavior
    {
        protected override async void OnOpen()
        {
            #region Update User p2p SID
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
            currentUserMappings.P2PSid = ID;

            updateResult = await this.UpdateUserMapping(currentUserMappings);

            if (updateResult)
                logger.Logger.Information("L'utilisateur [{0}] a établi une nouvelle connexion p2p | [ID]: {1}", currentUser.UserName, ID);
            else
                logger.Logger.Fatal("Une erreur est survenue lors de l'ouverture de la connexion Source: {0}", typeof(FileMessageHeadBehavior).FullName);

            #endregion 
        }
        protected override async void OnMessage(MessageEventArgs e)
        {
            if (e.IsText)
            {
                var textMessage = Newton.DeserializeObject<Lib.TextMessage>(e.Data);
                var mapping = await this.GetSpecifiedUserSessionMappings(textMessage.RecipientId);

                if (mapping != null)
                {
                    if (isSessionActive(mapping.P2PSid))
                    {
                        Sessions.SendTo(e.Data, mapping.P2PSid);
                        logger.Logger.Information("Transfert [Texte]: {0} ==> {1} | {2} octets", ID, mapping.P2PSid, e.RawData.Length);
                        return;
                    }
                }
                logger.Logger.Fatal("Une erreur est survenue | Impossible de router le message vers le destinataire spécifié");
            }
            else
                CloseAsync(CloseStatusCode.UnsupportedData, "Bad request");
        }
        protected override void OnClose(CloseEventArgs e)
        {
            if (e.WasClean)
                logger.Logger.Verbose("Connexion Fermée!");
            else
                logger.Logger.Fatal("La connexion {0} a été terminée pour la raison: {1} | Source: {2}", ID, e.Reason, typeof(UserMessagingServiceBehavior).FullName);
        }
        protected override void OnError(ErrorEventArgs e)
        {
            logger.Logger.Error<ErrorEventArgs>(e.Exception, "Connexion a été explicitement terminée due à une erreur", e);
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
