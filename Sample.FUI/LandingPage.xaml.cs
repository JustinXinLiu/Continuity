using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Continuity.Extensions;

namespace Sample.FUI
{
    public sealed partial class LandingPage : Page
    {
        public LandingPage()
        {
            InitializeComponent();

            //Loaded += async (s, e) =>
            //{
            //    FindName("MyRectangle");
            //    MyRectangle.EnableFluidVisibilityAnimation();
            //    MyRectangle.Visibility = Visibility.Collapsed;
            //    await Task.Delay(2000);

            //    MyRectangle.Visibility = Visibility.Visible;
            //};
        }
    }
}