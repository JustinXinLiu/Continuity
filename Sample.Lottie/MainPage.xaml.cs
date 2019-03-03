using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Continuity.Controls;
using Microsoft.UI.Xaml.Controls;
using Continuity.Extensions;
using System.Linq;

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
                var player = toggle.Children().OfType<AnimatedVisualPlayer>().Single();

                toggle.Checked += async (s, e) =>
                {
                    if (!player.IsPlaying)
                    {
                        player.Opacity = 1;
                        player.AutoPlay = true;
                    }

                    player.Resume();
                };
                toggle.Unchecked += (s, e) =>
                {
                    player.Pause();
                };
            }
        }
    }
}