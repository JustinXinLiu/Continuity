using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Template10.Common;

namespace Sample.Main
{
    sealed partial class App : BootStrapper
    {
        public App()
        {
            InitializeComponent();

            UnhandledException += OnAppUnhandledException;
        }

        private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
        }

        public override Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            NavigationService.Navigate(typeof(Views.PlayersView), "Runtime value");
            return Task.CompletedTask;
        }
    }
}