using Sample.Main.ViewModels;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Sample.Main.Views
{
    public sealed partial class PlayersView : Page
    {
        public PlayersView()
        {
            InitializeComponent();

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(360, 640));
        }

        public PlayersViewModel Vm => DataContext as PlayersViewModel;

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Frame.Navigate(typeof(PlayerView));
        }
    }
}
