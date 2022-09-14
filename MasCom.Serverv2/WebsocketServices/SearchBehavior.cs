using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System.Data;
using System.IO;
using System.Linq;
using WebSocketSharper;
using WebSocketSharper.Server;
using logger = Serilog.Log;

namespace MasCom.Serverv2.WebsocketServices
{
    public class SearchBehavior : WebSocketBehavior
    {
        private readonly string ConnectionString;
        public SearchBehavior()
        {
            IConfigurationRoot _config = new ConfigurationBuilder()
                             .SetBasePath(Directory.GetCurrentDirectory())
                             .AddJsonFile("appsettings.json")
                             .Build();

            ConnectionString = _config.GetConnectionString("MysqlDb");
        }


        protected override void OnOpen()
        {
            logger.Logger.Information("Scan du réseau....");
            logger.Logger.Information("Recherche d'utilisateurs....");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            logger.Logger.Information("Requête de la Liste des utilisateurs..");

            using IDbConnection db = new MySqlConnection(ConnectionString);
            db.Open();
            System.Collections.Generic.List<Lib.User> users = db.GetList<Lib.User>().ToList();
            string serializedUsers = Newtonsoft.Json.JsonConvert.SerializeObject(users);

            if (isSessionActive(ID))
            {
                Send(serializedUsers);
                logger.Logger.Information("Delivered User'");
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
