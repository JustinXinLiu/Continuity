using Windows.UI.Xaml.Controls;
using Continuity.Extensions;
using Windows.UI.Xaml;
using System.Numerics;
using Continuity;

namespace Sample.KlivaDesign.Controls
{
	public sealed partial class SpeedGauge : UserControl
	{
		public SpeedGauge()
		{
			InitializeComponent();

			Gauge.EnableFluidVisibilityAnimation(showFromScale: 0.2f, hideToScale: 0.2f, showDuration: 400, hideDuration: 400);
			Icon.EnableFluidVisibilityAnimation(showDuration: 400, hideDuration: 400);
			Caption.EnableFluidVisibilityAnimation(showDuration: 400, hideDuration: 400);
			MaxValuePanel.EnableFluidVisibilityAnimation(showDuration: 400, hideDuration: 400);
			MaxValueText.EnableFluidVisibilityAnimation(AnimationAxis.X, showFromOffset: -8.0f, hideToOffset: -8.0f, showDuration: 400, hideDuration: 400);

			AvgValuePanel.EnableImplicitAnimation(VisualPropertyType.Offset, duration: 800);
			AvgValueWrapper.EnableImplicitAnimation(VisualPropertyType.Offset, duration: 400);
		}

		public double PercentValue
		{
			get => (double)GetValue(PercentValueProperty);
			set => SetValue(PercentValueProperty, value);
		}
		public static readonly DependencyProperty PercentValueProperty =
			DependencyProperty.Register("PercentValue", typeof(double), typeof(SpeedGauge), new PropertyMetadata(0.0d));

		public double MaxValue
		{
			get => (double)GetValue(MaxValueProperty);
			set => SetValue(MaxValueProperty, value);
		}
		public static readonly DependencyProperty MaxValueProperty =
			DependencyProperty.Register("MaxValue", typeof(double), typeof(SpeedGauge), new PropertyMetadata(0.0d));

		public double AverageValue
		{
			get => (double)GetValue(AverageValueProperty);
			set => SetValue(AverageValueProperty, value);
		}
		public static readonly DependencyProperty AverageValueProperty =
			DependencyProperty.Register("AverageValue", typeof(double), typeof(SpeedGauge), new PropertyMetadata(0.0d));
	}
}
