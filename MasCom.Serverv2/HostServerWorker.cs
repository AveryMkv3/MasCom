using Dapper;
using MasCom.Serverv2.WebsocketServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Serilog;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharper.Net;
using WebSocketSharper.Server;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace MasCom.Serverv2
{
    public class HostServerWorker : BackgroundService
    {
        private readonly HttpServer mainHttpServer;
        private readonly IConfiguration configuration;
        private readonly ILogger logger = new LoggerFactory()
                                                        .AddSerilog(Log.ForContext<Program>())
                                                        .CreateLogger<Program>();

        public HostServerWorker(IConfiguration _configuration)
        {
            configuration = _configuration;

            int port = _configuration.GetValue<int>("ServerConfig:Port");

            mainHttpServer = new HttpServer(logger, port, false) { KeepClean = true };

            Log.Logger.Verbose("Démarrage Des Services de Messagerie");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            #region SSL Configuration
            //mainHttpServer.SslConfiguration.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2("visio-cert.pfx", "123456789");
            #endregion

            #region Realm Settings
            mainHttpServer.Realm = $"{Environment.MachineName}::{Environment.UserDomainName}";
            Log.Logger.Information("Domaine: {0}", mainHttpServer.Realm);
            #endregion

            #region Authentication Configuration

            //-- Auth. Scheme
            mainHttpServer.AuthenticationSchemes = AuthenticationSchemes.Basic;

            Log.Logger.Warning("Activation des Services d'Authentification");

            //-- Credential validator
            mainHttpServer.UserCredentialsFinder = (Identity) =>
            {
                var name = Identity.Name;
                Log.Logger.Information("Vérification des informations d'authentifiation...");

                Lib.User user = null;
                var _conStr = configuration.GetConnectionString("MysqlDb");
                using (IDbConnection db = new MySqlConnection(_conStr))
                {
                    db.Open();
                    user = db.GetList<Lib.User>().ToList().Where(u => string
                                                                    .Compare(u.UserName, name,
                                                                        StringComparison.OrdinalIgnoreCase) == 0)
                                                                            .FirstOrDefault();
                }

                if (user == null)
                {
                    Log.Logger.Error("Impossible d'authentifier l'utilisateur");
                    return null;
                }
                else
                {
                    Log.Logger.Warning("Authentification En cours pour {0}...", user.UserName);
                    return new NetworkCredential(name, user.PasswordHash);
                }
            };
            #endregion

            Log.Logger.Information("Ajout des points terminaux pour les services websocket...");

            mainHttpServer.AddWebSocketService<AuthenticateBehavior>("/authenticate");

            mainHttpServer.AddWebSocketService<UserMessagingServiceBehavior>("/message_user");

            mainHttpServer.AddWebSocketService<FileMessageHeadBehavior>("/message_file_headers");
            mainHttpServer.AddWebSocketService<FileDeliveryServiceBehavior>("/deliver_file");

            mainHttpServer.AddWebSocketService<GroupMessagingBehavior>("/message_group");

            mainHttpServer.AddWebSocketService<SearchBehavior>("/search");

            mainHttpServer.AddWebSocketService<VideoConferenceBehavior>("/videoconf");
            mainHttpServer.AddWebSocketService<VisioCallService>("/call_user");


            mainHttpServer.AddWebSocketService<AudioConferenceBehavior>("/audioconf");


            //-- load additional routes by plugin
            Log.Logger.Information("Chargement dynamique des points terminaux supplémentaires");
            await Task.Delay(TimeSpan.FromSeconds(2));

            //-- output configured routes
            Log.Logger.Information("Points Terminaux configurés: ");
            mainHttpServer.WebSocketServices.Paths.ToList().ForEach(p => Log.Logger.Information(" ► {0}", p));

            #region Incomming Http Request OnGet
            mainHttpServer.OnGet += (sender, e) =>
            {
                ResolveFileRequest(e, e.Request.Url.AbsoluteUri);
            };
            #endregion

            #region ContentRoot Path setting for http server
            var contentrootPath = Path.Combine(Environment.CurrentDirectory, "wwwroot");
            var fileTransfertPath = Path.Combine(contentrootPath, "ft");
            if (!Directory.Exists(contentrootPath)) Directory.CreateDirectory(contentrootPath);
            if (!Directory.Exists(fileTransfertPath)) Directory.CreateDirectory(fileTransfertPath);
            mainHttpServer.DocumentRootPath = contentrootPath;
            #endregion

            Log.Logger.Information("Racine du serveur: {0}", mainHttpServer.DocumentRootPath);

            mainHttpServer.Start();

            if (mainHttpServer.IsListening)
            {
                Log.Logger.Information("Server prêt sur le port {0} |", mainHttpServer.Port);
            }
            else
                mainHttpServer.Log.LogCritical("Une erreur s'est produite lors du démarrage du serveur..");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (mainHttpServer.IsListening)
                mainHttpServer.Stop();

            Log.Logger.Warning("Hôte du serveur arrêté!");
            await base.StopAsync(cancellationToken);
        }

        private void ResolveFileRequest(HttpRequestEventArgs e, string requestPath)
        {
            var possibleFiles = Directory.EnumerateFiles("wwwroot", "*.*", SearchOption.AllDirectories).ToList();

            var fn = Path.GetFileName(requestPath);
            if (!string.IsNullOrWhiteSpace(fn))
            {
                for (int i = 0; i < possibleFiles.Count; i++)
                {
                    var formatedRequestpath = string.Join("\\", requestPath.Split("/"));
                    var relativePath = Path.GetRelativePath("wwwroot", possibleFiles[i]);

                    if (formatedRequestpath.Contains(relativePath))
                    {
                        Log.Logger.Information("File found");

                        var file = e.ReadFile(relativePath);
                        e.Response.ContentEncoding = Encoding.UTF8;
                        e.Response.ContentLength64 = (long)file.Length;
                        e.Response.Close(file, true);

                        file = Enumerable.Empty<byte>().ToArray();
                        Log.Logger.Information("{0} octets servis", e.Response.ContentLength64);
                        break;
                    }
                }
            }

            e.Response.Close();
        }
    }
}
