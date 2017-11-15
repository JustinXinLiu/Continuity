using Windows.UI.Xaml.Controls;
using Continuity.Extensions;
using Windows.UI.Xaml;
using Continuity;
using System.Numerics;

namespace Sample.KlivaDesign.Controls
{
	public sealed partial class DistanceGauge : UserControl
	{
		public DistanceGauge()
		{
			InitializeComponent();

			DistanceLine.EnableFluidVisibilityAnimation(showFromScale: 0.4f, hideToScale: 0.4f, showDuration: 400, hideDuration: 400);
			ElevationPanel.EnableFluidVisibilityAnimation(showDuration: 400, hideDuration: 400);
			TimeCaption.EnableFluidVisibilityAnimation(showDuration: 400, hideDuration: 400);
			DistanceCaption.EnableFluidVisibilityAnimation(showDuration: 400, hideDuration: 400);

			TimeCaption.EnableImplicitAnimation(VisualPropertyType.Offset, duration: 400);
			TimePanel.EnableImplicitAnimation(VisualPropertyType.Offset, duration: 400);
			DistanceCaption.EnableImplicitAnimation(VisualPropertyType.Offset, duration: 400);
			DistancePanel.EnableImplicitAnimation(VisualPropertyType.Offset, duration: 400);
		}

		public string MovingTime
		{
			get => (string)GetValue(MovingTimeProperty);
			set => SetValue(MovingTimeProperty, value);
		}
		public static readonly DependencyProperty MovingTimeProperty =
			DependencyProperty.Register("MovingTime", typeof(string), typeof(DistanceGauge), new PropertyMetadata(0.0d));

		public double TotalDistance
		{
			get => (double)GetValue(TotalDistanceProperty);
			set => SetValue(TotalDistanceProperty, value);
		}
		public static readonly DependencyProperty TotalDistanceProperty =
			DependencyProperty.Register("TotalDistance", typeof(double), typeof(DistanceGauge), new PropertyMetadata(0.0d));

		public double ElevationGain
		{
			get => (double)GetValue(ElevationGainProperty);
			set => SetValue(ElevationGainProperty, value);
		}
		public static readonly DependencyProperty ElevationGainProperty =
			DependencyProperty.Register("ElevationGain", typeof(double), typeof(DistanceGauge), new PropertyMetadata(0.0d));

		private void OnDistanceLineSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.PreviousSize.Equals(e.NewSize)) return;

			DistanceLine.Visual().CenterPoint = new Vector3(DistanceLine.RenderSize.ToVector2() / 2, 0.0f);
		}
	}
}