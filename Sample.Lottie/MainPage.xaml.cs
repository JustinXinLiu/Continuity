using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Continuity.Controls;

namespace Sample.Lottie
{
    public sealed partial class MainPage : Page
    {
        private List<FlipToggleButton> Toggles { get; } = new List<FlipToggleButton>();

        public MainPage()
        {
            InitializeComponent();

            PopulateToggles();
            void PopulateToggles()
            {
                Toggles.Add(L);
                Toggles.Add(O);
                Toggles.Add(T1);
                Toggles.Add(T2);
                Toggles.Add(I);
                Toggles.Add(E);
            }
        }

        private async void OnGoClick(object sender, RoutedEventArgs e) => 
            await FlipToggleButtonsAsync();

        private async Task FlipToggleButtonsAsync(int duration = 50)
        {
            foreach (var toggle in Toggles)
            {
                await Task.Delay(duration);
                toggle.IsChecked = false;
            }

            foreach (var toggle in Toggles)
            {
                toggle.Checked += (s,e) =>
                {

                };
                toggle.Unchecked += (s, e) =>
                {

                };
            }
        }
    }
}