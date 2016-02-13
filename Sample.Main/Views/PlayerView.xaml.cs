using Sample.Main.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Sample.Main.Views
{
    public sealed partial class PlayerView : Page
    {
        public PlayerView()
        {
            InitializeComponent();
        }

        public PlayerViewModel Vm => DataContext as PlayerViewModel;
    }
}
