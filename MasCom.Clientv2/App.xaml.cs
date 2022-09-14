using GalaSoft.MvvmLight.Ioc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Windows;

namespace MasCom.Clientv2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IConfiguration Configuration { get; set; }

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            App.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            Configuration = new ConfigurationBuilder()
                                        .SetBasePath(Directory.GetCurrentDirectory())
                                        .AddJsonFile("Config.json", optional: false)
                                        .Build();


            SimpleIoc.Default.Register<Connexion>();
            SimpleIoc.Default.Register<MainWindow>();

            App.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            var appDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MasCom");
            if (!Directory.Exists(appDir)) Directory.CreateDirectory(appDir);

            var filesDirectory = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MasCom", "Fichiers");
            if (!Directory.Exists(filesDirectory)) Directory.CreateDirectory(filesDirectory);

            AppDomain.CurrentDomain.SetData("appDir", appDir);

        }


        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Une erreur critique s'est produite", 
                            "Erreur Fatale", 
                            MessageBoxButton.OK, 
                            MessageBoxImage.Error);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Une erreur critique s'est produite",
                           "Erreur Fatale",
                           MessageBoxButton.OK,
                           MessageBoxImage.Error);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Connexion connexionUI = SimpleIoc.Default.GetInstanceWithoutCaching<Connexion>();
            this.MainWindow = connexionUI;
            connexionUI.Show();
        }
    }
}
