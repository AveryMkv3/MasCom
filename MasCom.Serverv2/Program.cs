using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace MasCom.Serverv2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Dapper.SimpleCRUD.SetDialect(Dapper.SimpleCRUD.Dialect.MySQL);

            Log.Logger = new LoggerConfiguration()
                                .WriteTo
                                .Console(LogEventLevel.Information)
                                .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureLogging(lb => lb.AddSerilog());

                    webBuilder.UseStartup<Startup>();

                    webBuilder.UseUrls(new string[] { "http://localhost:9796" });
                    webBuilder.PreferHostingUrls(true);

                    Log.Logger.Information("Vérification de la connexion à la base de données...");
                    Log.Logger.Information("Ouverture - OK");
                    Log.Logger.Information("Vérification Terminée");
                });
    }
}
