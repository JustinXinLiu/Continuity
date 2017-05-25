using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Sample.Navigation
{
    public sealed partial class AppsPage : Page
    {
        public ObservableCollection<Color> Colors { get; } = new ObservableCollection<Color>();

        public AppsPage()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                foreach (var color in typeof(Colors).GetRuntimeProperties().Select(x => (Color)x.GetValue(null)))
                {
                    Colors.Add(color);
                }
            };
        }
    }
}
