using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using Sample.Main.Services;
using Sample.Main.ViewModels;

namespace Sample.Main.Mvvm
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            
            // TODO: uncomment this for real service calls
            //if (ViewModelBase.IsInDesignModeStatic)
            //{
            SimpleIoc.Default.Register<IPlayerService, DesignTimePlayerService>();
            //}
            //else
            //{
            //    SimpleIoc.Default.Register<IPlayerService, PlayerService>();
            //}

            SimpleIoc.Default.Register<PlayersViewModel>();
            SimpleIoc.Default.Register<PlayerViewModel>();
        }

        public PlayersViewModel Players => ServiceLocator.Current.GetInstance<PlayersViewModel>();

        public PlayerViewModel Player => ServiceLocator.Current.GetInstance<PlayerViewModel>();
    }
}
