using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Continuity.Extensions;
using System.Collections.Generic;

namespace Sample.KlivaDesign
{
    public sealed partial class LandingPage : Page
    {
        #region Fields

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

        private async void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Height.Equals(e.NewSize.Height)) return;

            var sizeChangedTasks = new List<Task>();

            foreach (Border viewHost in ViewsContainer.Children)
            {
                var task = viewHost.SizeChangedAsync();
                sizeChangedTasks.Add(task);

                viewHost.Height = e.NewSize.Height;
            };

            await Task.WhenAll(sizeChangedTasks);

            SyncScrollPosition();

            void SyncScrollPosition()
            {
                if (ActivityMenuItem != null && ActivityMenuItem.IsChecked.Value)
                {
                    MainScrollViewer.ChangeView(null, e.NewSize.Height, null);
                }
                else if (StatsMenuItem != null && StatsMenuItem.IsChecked.Value)
                {
                    MainScrollViewer.ChangeView(null, e.NewSize.Height * 2, null);
                }
                else if (AccountMenuItem != null && AccountMenuItem.IsChecked.Value)
                {
                    MainScrollViewer.ChangeView(null, e.NewSize.Height * 3, null);
                }
                else if (SettingsMenuItem != null && SettingsMenuItem.IsChecked.Value)
                {
                    MainScrollViewer.ChangeView(null, e.NewSize.Height * 4, null);
                }
            }
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

            MainScrollViewer.ChangeView(null, Window.Current.Bounds.Height * positionIndex, null, false);
        }

        #endregion
    }
}