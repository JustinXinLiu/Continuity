using Sample.Main.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Sample.Main.Views
{
    public sealed partial class PlayersView : Page
    {
        public PlayersView()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
        }

        public PlayersViewModel Vm => DataContext as PlayersViewModel;
    }
}
