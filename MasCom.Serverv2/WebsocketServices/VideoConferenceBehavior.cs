using WebSocketSharper;
using WebSocketSharper.Server;
using logger = Serilog.Log;
using System.Threading.Tasks;

namespace MasCom.Serverv2.WebsocketServices
{
    public class VideoConferenceBehavior : WebSocketBehavior
    {
        protected override void OnOpen()
        {
            logger.Logger.Information("Un nouvel utilisateur a rejoint la discussion | {0}", ID);
            logger.Logger.Information("Nombre de Sessions: {0}", Sessions.Count);
        }

        protected override async void OnMessage(MessageEventArgs e)
        {
            Sessions.BroadcastAsync(e.RawData, () => { });
            await Task.CompletedTask;
        }

        protected override void OnClose(CloseEventArgs e)
        {
            logger.Logger.Warning("Utilisateur déconnecté");
        }

        protected override void OnError(ErrorEventArgs e)
        {
            logger.Logger.Error("Une erreur s'est produite");
        }
    }
}
