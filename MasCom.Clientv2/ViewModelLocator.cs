using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;

namespace MasCom.Clientv2
{
    class ViewModelLocator : ICleanup
    {
        public ViewModels.MainViewModel MainVM => SimpleIoc.Default.GetInstance<ViewModels.MainViewModel>();
        public ViewModels.AuthenticationVM AuthVM => SimpleIoc.Default.GetInstanceWithoutCaching<ViewModels.AuthenticationVM>();
        public ViewModels.VisioWindowVM VisioVM => SimpleIoc.Default.GetInstanceWithoutCaching<ViewModels.VisioWindowVM>();
        public ViewModels.SettingsVM SettingsVM => SimpleIoc.Default.GetInstanceWithoutCaching<ViewModels.SettingsVM>();

        public Lib.User LoggedUser => AppDomain.CurrentDomain.GetData("user") as Lib.User;

        public ViewModelLocator()
        {
            SimpleIoc.Default.Register<ViewModels.MainViewModel>();
            SimpleIoc.Default.Register<ViewModels.AuthenticationVM>();
            SimpleIoc.Default.Register<ViewModels.VisioWindowVM>();
            SimpleIoc.Default.Register<ViewModels.SettingsVM>();
        }

        public void Cleanup()
        {
            //ToDo: Clean
        }
    }
}
