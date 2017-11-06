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

        private const int RenderingDelay = 200;

		#endregion

        public LandingPage()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e) =>
            await LoginAsync();

        #region Login

        private async Task LoginAsync()
        {
            if (!IsUserLoggedIn())
            {
                FindName(nameof(LoginView));
                await Task.Delay(1500);
            }

            await ShowTopPanelAsync();

            await ScrollToViewAsync(nameof(ActivityView), ActivityView, ActivityHostView);
            ActivityMenuItem.IsChecked = true;

			
		}

        private bool IsUserLoggedIn()
        {
            var random = new Random();
            int probability = random.Next(100);
            return probability <= 1;
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
            await ScrollToViewAsync(nameof(ActivityView), ActivityView, ActivityHostView);

        private async void OnStatsMenuItemChecked(object sender, RoutedEventArgs e) =>
            await ScrollToViewAsync(nameof(StatsView), StatsView, StatsHostView);

        private async void OnAccountMenuItemChecked(object sender, RoutedEventArgs e) =>
            await ScrollToViewAsync(nameof(AccountView), AccountView, AccountHostView);

        private async void OnSettingsMenuItemChecked(object sender, RoutedEventArgs e) =>
            await ScrollToViewAsync(nameof(SettingsView), SettingsView, SettingsHostView);

        #endregion

        #region Miscs

        private async Task ScrollToViewAsync(string viewName, UserControl view, FlipViewItem nextHostView)
        {
            if (view == null)
            {
                FindName(viewName);
                await Task.Delay(RenderingDelay);
            }

            var currentViewIndex = HostFlipView.SelectedIndex;
            var nextViewIndex = HostFlipView.IndexFromContainer(nextHostView);

            if (currentViewIndex < nextViewIndex)
            {
                for (var i = currentViewIndex; i < nextViewIndex; i++)
                {
                    HostFlipView.SelectedIndex += 1;
                    await Task.Yield();
                }
            }
            else
            {
                for (var i = nextViewIndex; i < currentViewIndex; i++)
                {
                    HostFlipView.SelectedIndex -= 1;
                    await Task.Yield();
                }
            }
        }

        #endregion
    }
}