using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sample.KlivaDesign
{
    public sealed partial class LandingPage : Page
    {
        public static double WindowWidth { get; } = Window.Current.Bounds.Width;
        public static double WindowHeight { get; } = Window.Current.Bounds.Height;

        public LandingPage()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e) =>
            await LoginAsync();

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width.Equals(e.NewSize.Width)) return;

            foreach (Border viewHost in ViewsContainer.Children)
            {
                viewHost.Width = WindowWidth;
                viewHost.Height = WindowHeight;
            };
        }

        #region Login

        private async Task LoginAsync()
        {
            await Task.Delay(2000);

            await ShowTopPanelAsync();

            await ScrollToViewAsync(nameof(ActivityView), ActivityView, 1);
            ActivityMenuItem.IsChecked = true;
        }

        #endregion

        #region TopPanel

        private async Task ShowTopPanelAsync()
        {
            if (TopPane == null)
            {
                FindName(nameof(TopPane));
                //TopPane.EnableFluidVisibilityAnimation(showFromOffset: -100.0f, hideToOffset: 100.0f, showDuration: 1200, hideDuration: 400);
                TopPane.Visibility = Visibility.Collapsed;
                await Task.Yield();
            }

            TopPane.Visibility = Visibility.Visible;
        }

        private void HideTopPanel() => TopPane.Visibility = Visibility.Collapsed;

        #endregion

        #region Menu

        private async void OnActivityMenuItemChecked(object sender, RoutedEventArgs e) =>
            await ScrollToViewAsync(nameof(ActivityView), ActivityView, 1);

        private async void OnStatsMenuItemChecked(object sender, RoutedEventArgs e) =>
            await ScrollToViewAsync(nameof(StatsView), StatsView, 2);

        private async void OnAccountMenuItemChecked(object sender, RoutedEventArgs e) =>
            await ScrollToViewAsync(nameof(AccountView), AccountView, 3);

        private async void OnSettingsMenuItemChecked(object sender, RoutedEventArgs e) =>
            await ScrollToViewAsync(nameof(SettingsView), SettingsView, 4);

        #endregion

        #region Miscs

        private async Task ScrollToViewAsync(string viewName, UserControl view, int positionIndex = 0)
        {
            if (view == null)
            {
                FindName(viewName);
                await Task.Delay(400);
            }

            MainScrollViewer.ChangeView(null, WindowHeight * positionIndex, null, false);
        }

        #endregion
    }
}