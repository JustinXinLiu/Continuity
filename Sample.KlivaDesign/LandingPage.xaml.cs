using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sample.KlivaDesign
{
    public sealed partial class LandingPage : Page
    {
        public double WindowHeight { get; } = Window.Current.Bounds.Height;

        public LandingPage()
        {
            InitializeComponent();

            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width.Equals(e.NewSize.Width)) return;

            foreach (UserControl view in ViewsContainer.Children)
            {
                view.Width = Window.Current.Bounds.Width;
                view.Height = Window.Current.Bounds.Height;
            }
        }

        #region Menu

        private void OnActivityMenuItemChecked(object sender, RoutedEventArgs e)
        {
            MainScrollViewer.ChangeView(null, 0, null, false);
        }

        private void OnStatsMenuItemChecked(object sender, RoutedEventArgs e)
        {
            MainScrollViewer.ChangeView(null, WindowHeight, null, false);
        }

        private void OnAccountMenuItemChecked(object sender, RoutedEventArgs e)
        {
            MainScrollViewer.ChangeView(null, WindowHeight * 2, null, false);
        }

        private void OnSettingsMenuItemChecked(object sender, RoutedEventArgs e)
        {
            MainScrollViewer.ChangeView(null, WindowHeight * 3, null, false);
        }

        #endregion
    }
}