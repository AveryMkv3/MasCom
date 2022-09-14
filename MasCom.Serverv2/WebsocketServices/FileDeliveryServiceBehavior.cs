using Newtonsoft.Json;
using System.IO;
using WebSocketSharper;
using WebSocketSharper.Server;
using logger = Serilog.Log;

namespace MasCom.Serverv2.WebsocketServices
{
    public class FileDeliveryServiceBehavior : WebSocketBehavior
    {
        protected override async void OnOpen()
        {
            string deliveryId = QueryString["delivery_id"];
            #region Update User File Delivery SID if Not a headered delivery
            if (string.IsNullOrWhiteSpace(deliveryId))
            {
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
                currentUserMappings.FileDeliverySid = ID;

                updateResult = await this.UpdateUserMapping(currentUserMappings);

                if (updateResult)
                    logger.Logger.Information("Service de Transfert de fichier | [ID]: {0}", ID);
                else
                    logger.Logger.Fatal("Une erreur est survenue lors de l'ouverture de la connexion | Source: {0}", typeof(FileMessageHeadBehavior).FullName);

            }
            #endregion
        }
        protected override async void OnMessage(MessageEventArgs e)
        {
            string deliveryId = QueryString["delivery_id"];

            if (string.IsNullOrWhiteSpace(deliveryId))
            {
                logger.Error("Impossible de délivrer le message, l'adresse de livraison est incorrecte");
                Close();
                return;
            }

            if (e.IsBinary)
            {
                //Check delivery details
                var fmH = await this.GetAssociatedDeliveryHeaders(deliveryId);

                if (fmH == null)
                {
                    logger.Error("Impossible de délivrer le message, l'adresse de livraison est incorrecte");
                    Close();
                    return;
                }

                //Sauvegarde Locale
                var savedFilePath = Path.Join("wwwroot", "ft", $"{fmH.DeliveryId}{fmH.Extension}");
                await File.WriteAllBytesAsync(savedFilePath, e.RawData);
                logger.Information("Fichier Reçu. La sauvegarde locale est prête à être délivré");

                var recipientSessionMappings = await this.GetSpecifiedUserSessionMappings(fmH.RecipientId);

                fmH.FileFetchUrl = Path.GetRelativePath("wwwroot", savedFilePath);

                if (recipientSessionMappings != null)
                {
                    var response = JsonConvert.SerializeObject(fmH);

                    if(isSessionActive(recipientSessionMappings.FileDeliverySid))
                        Sessions.SendTo(response, recipientSessionMappings.FileDeliverySid);
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
