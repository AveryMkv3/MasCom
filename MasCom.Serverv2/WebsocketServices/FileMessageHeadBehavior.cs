using System;
using WebSocketSharper;
using WebSocketSharper.Server;
using logger = Serilog.Log;

namespace MasCom.Serverv2.WebsocketServices
{
    public class FileMessageHeadBehavior : WebSocketBehavior
    {
        protected override async void OnOpen()
        {
            #region Update File Headers SID
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
            currentUserMappings.FileHeadersDeliverySid = ID;

            updateResult = await this.UpdateUserMapping(currentUserMappings);

            if (updateResult)
                logger.Logger.Information("Préparation de l'envoi d'en-têtes de fichier | [ID]: {0}", ID);
            else
                logger.Logger.Fatal("Une erreur est survenue lors de l'ouverture de la connexion Source: {0}", typeof(FileMessageHeadBehavior).FullName);
            #endregion 
        }
        protected async override void OnMessage(MessageEventArgs e)
        {
            if (e.IsText)
            {
                var fileMessageHeaders = Newtonsoft.Json.JsonConvert.DeserializeObject<Lib.FileMessageHeaders>(e.Data);
                if (fileMessageHeaders != null)
                {
                    fileMessageHeaders.DeliveryId = Guid.NewGuid().ToString()[..6];

                    Lib.FileDeliveryInfo fdI = new Lib.FileDeliveryInfo()
                    {
                        FilePath = fileMessageHeaders.FileName,
                        DeliveryId = fileMessageHeaders.DeliveryId
                    };

                    fileMessageHeaders.FileName = System.IO.Path.GetFileNameWithoutExtension(fileMessageHeaders.FileName);

                    int id = (await this.SaveFileMessageHeadersToDatabase(fileMessageHeaders)).GetValueOrDefault(0);

                    if (id != 0)
                    {
                        var response = Newtonsoft.Json.JsonConvert.SerializeObject(fdI);
                        if(isSessionActive(ID))
                            Send(response);
                    }
                }
            }
        }
        protected override void OnClose(CloseEventArgs e)
        {
            if (e.WasClean)
                logger.Logger.Verbose("Connexion Fermée!");
            else
                logger.Logger.Fatal("La connexion {0} a été terminée pour la raison: {1} | Source: {2}", ID, e.Reason, typeof(FileMessageHeadBehavior).FullName);
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
