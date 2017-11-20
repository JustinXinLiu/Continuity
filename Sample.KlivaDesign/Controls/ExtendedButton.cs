using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Continuity.Controls
{
	public sealed class ExtendedButton : Button
	{
		public CornerRadius CornerRadius
		{
			get => (CornerRadius)GetValue(CornerRadiusProperty);
			set => SetValue(CornerRadiusProperty, value);
		}
		public static readonly DependencyProperty CornerRadiusProperty =
			DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ExtendedButton), new PropertyMetadata(null));

		public Style IconStyle
		{
			get => (Style)GetValue(IconStyleProperty);
			set => SetValue(IconStyleProperty, value);
		}
		public static readonly DependencyProperty IconStyleProperty =
			DependencyProperty.Register("IconStyle", typeof(Style), typeof(ExtendedButton), new PropertyMetadata(null));

		public ExtendedButton()
		{
			DefaultStyleKey = typeof(ExtendedButton);
		}
	}
}