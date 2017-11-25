using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

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

		public Brush IconForeground
		{
			get => (Brush)GetValue(IconForegroundProperty);
			set => SetValue(IconForegroundProperty, value);
		}
		public static readonly DependencyProperty IconForegroundProperty =
			DependencyProperty.Register("IconForeground", typeof(Brush), typeof(ExtendedButton), new PropertyMetadata(null));

		public Thickness IconMargin
		{
			get => (Thickness)GetValue(IconMarginProperty);
			set => SetValue(IconMarginProperty, value);
		}
		public static readonly DependencyProperty IconMarginProperty =
			DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(ExtendedButton), new PropertyMetadata(null));


		public ExtendedButton()
		{
			DefaultStyleKey = typeof(ExtendedButton);
		}
	}
}