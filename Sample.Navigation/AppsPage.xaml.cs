using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace Sample.Navigation
{
    public sealed partial class AppsPage : Page
    {
        public AppsPage()
        {
            InitializeComponent();

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(200, 200));
            ApplicationView.PreferredLaunchViewSize = new Size(640, 640);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        private IEnumerable<Color> Colors => typeof(Colors).GetRuntimeProperties().Select(x => (Color) x.GetValue(null));
    }
}
