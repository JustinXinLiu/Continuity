using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Continuity.Extensions;

namespace Sample.KlivaDesign
{
    public sealed partial class LandingPage : Page
    {
        #region Fields

        public static double WindowWidth { get; } = Window.Current.Bounds.Width;
        public static double WindowHeight { get; } = Window.Current.Bounds.Height;

        private const int RenderingDelay = 400;

        #endregion

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
            if (!IsUserLoggedIn())
            {
                FindName(nameof(LoginView));
                await Task.Delay(1500);
            }

            await ShowTopPanelAsync();

            await ScrollToViewAsync(nameof(ActivityView), ActivityView, 1);
            ActivityMenuItem.IsChecked = true;
        }

        private bool IsUserLoggedIn()
        {
            var random = new Random();
            int probability = random.Next(100);
            return probability <= 80;
        }

        #endregion

        #region TopPanel

        private async Task ShowTopPanelAsync()
        {
            if (TopPane == null)
            {
                FindName(nameof(TopPane));
                TopPane.EnableFluidVisibilityAnimation(showDuration: 1600, hideDuration: 400);
                TopPane.Visibility = Visibility.Collapsed;
                await Task.Delay(RenderingDelay);
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
                await Task.Delay(RenderingDelay);
            }

            MainScrollViewer.ChangeView(null, WindowHeight * positionIndex, null, false);
        }

        #endregion
    }
}