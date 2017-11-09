using Windows.UI.Xaml.Controls;
using Continuity.Extensions;
using Windows.UI.Xaml;

namespace Sample.KlivaDesign.Controls
{
	public sealed partial class DistancePanel : UserControl
    {
        public DistancePanel()
        {
			InitializeComponent();
        }

		public string MovingTime
		{
			get => (string)GetValue(MovingTimeProperty);
			set => SetValue(MovingTimeProperty, value);
		}
		public static readonly DependencyProperty MovingTimeProperty =
			DependencyProperty.Register("MovingTime", typeof(string), typeof(DistancePanel), new PropertyMetadata(0.0d));

		public double TotalDistance
		{
			get => (double)GetValue(TotalDistanceProperty);
			set => SetValue(TotalDistanceProperty, value);
		}
		public static readonly DependencyProperty TotalDistanceProperty =
			DependencyProperty.Register("TotalDistance", typeof(double), typeof(DistancePanel), new PropertyMetadata(0.0d));

		public double ElevationGain
		{
			get => (double)GetValue(ElevationGainProperty);
			set => SetValue(ElevationGainProperty, value);
		}
		public static readonly DependencyProperty ElevationGainProperty =
			DependencyProperty.Register("ElevationGain", typeof(double), typeof(DistancePanel), new PropertyMetadata(0.0d));
	}
}
