using Continuity.Extensions;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sample.FUI
{
    public sealed partial class MainPage1 : Page
    {
        public MainPage1()
        {
            InitializeComponent();

            Loaded += OnMainPageLoaded;
        }

        private async void OnMainPageLoaded(object sender, RoutedEventArgs e)
        {
            var arcVisual = Arc.Visual();
            var compositor = arcVisual.Compositor;

            var clip = compositor.CreateInsetClip(0, 0, 80.0f, 80.0f);
            arcVisual.Clip = clip;

            await Task.Delay(2000);
            Shadow1.Opacity = 1;
            await Task.Delay(500);
            Shadow2.Opacity = 1;
            await Task.Delay(500);
            Shadow3.Opacity = 1;
            await Task.Delay(500);
            Shadow4.Opacity = 1;
            await Task.Delay(1500);
            Shadow5.Opacity = 1;
            await Task.Delay(500);
            Shadow5.Opacity = 0;
            await Task.Delay(25);
            Shadow5.Opacity = 1;
            await Task.Delay(25);
            Shadow5.Opacity = 0;
            await Task.Delay(25);
            Shadow5.Opacity = 1;
            await Task.Delay(25);
            Shadow5.Opacity = 0;
            await Task.Delay(1500);
            Shadow5.Opacity = 1;
        }
    }
}
