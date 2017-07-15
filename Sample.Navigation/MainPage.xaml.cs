using System.Linq;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Continuity.Controls;

namespace Sample.Navigation
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            //MaximizeWindowOnLoad();
            InitializeComponent();

            MyNavigationView.ItemInvoked += (s, e) =>
            {
                switch (e.InvokedItem)
                {
                    case "All Applications":
                        RootFrame.Navigate(typeof(AppsPage));
                        break;
                    case "Games":
                        RootFrame.Navigate(typeof(GamesPage));
                        break;
                    case "Calendar":
                        RootFrame.Navigate(typeof(CalendarPage));
                        break;
                    case "My Account":
                        RootFrame.Navigate(typeof(AccountPage));
                        break;
                }

                if (e.IsSettingsInvoked)
                {
                    RootFrame.Navigate(typeof(SettingsPage));
                }
            };

            // The following cannot be done in XAML in this build yet.
            MyNavigationView.AddMenuItem(Symbol.AllApps, "All Applications", true);
            MyNavigationView.AddMenuItem(Symbol.Video, "Games");
            MyNavigationView.AddMenuItem(Symbol.Calendar, "Calendar");
            MyNavigationView.AddMenuItemSeparator();
            MyNavigationView.AddMenuItem(Symbol.Admin, "My Account");

            RootFrame.Navigate(typeof(AppsPage));
        }

        private static void MaximizeWindowOnLoad()
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;

            ApplicationView.PreferredLaunchViewSize = new Size(bounds.Width, bounds.Height);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }
    }

    public static class NavigationViewExtensions
    {
        public static void AddMenuItem(this FluentNavigationView navigationView, Symbol icon, string text, bool isSelected = false)
        {
            var item = new FluentNavigationViewItem
            {
                Icon = new SymbolIcon(icon),
                Content = text
            };

            if (isSelected)
            {
                item.Loaded += OnItemLoaded;
                void OnItemLoaded(object sender, RoutedEventArgs e)
                {
                    item.Loaded -= OnItemLoaded;
                    item.IsSelected = true;
                }
            }           

            navigationView.MenuItems.Add(item);
        }

        public static void AddMenuItemSeparator(this NavigationView navigationView)
        {
            var item = new NavigationViewItemSeparator();
            navigationView.MenuItems.Add(item);
        }
    }
}
