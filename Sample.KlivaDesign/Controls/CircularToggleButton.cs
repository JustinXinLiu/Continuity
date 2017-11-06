using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Continuity.Controls
{
	public sealed class CircularToggleButton : ToggleButton
	{
		public object CheckedContent
		{
			get => GetValue(CheckedContentProperty);
			set => SetValue(CheckedContentProperty, value);
		}
		public static readonly DependencyProperty CheckedContentProperty =
			DependencyProperty.Register("CheckedContent", typeof(object), typeof(CircularToggleButton), new PropertyMetadata(null));

		public DataTemplate CheckedContentTemplate
		{
			get => (DataTemplate)GetValue(CheckedContentTemplateProperty);
			set => SetValue(CheckedContentTemplateProperty, value);
		}
		public static readonly DependencyProperty CheckedContentTemplateProperty =
			DependencyProperty.Register("CheckedContentTemplate", typeof(DataTemplate), typeof(CircularToggleButton), new PropertyMetadata(null));

		public CornerRadius CornerRadius
		{
			get => (CornerRadius)GetValue(CornerRadiusProperty);
			set => SetValue(CornerRadiusProperty, value);
		}
		public static readonly DependencyProperty CornerRadiusProperty =
			DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(CircularToggleButton), new PropertyMetadata(null));

		public CornerRadius CheckedCornerRadius
		{
			get => (CornerRadius)GetValue(CheckedCornerRadiusProperty);
			set => SetValue(CheckedCornerRadiusProperty, value);
		}
		public static readonly DependencyProperty CheckedCornerRadiusProperty =
			DependencyProperty.Register("CheckedCornerRadius", typeof(CornerRadius), typeof(CircularToggleButton), new PropertyMetadata(null));

		public Brush CheckedBackground
		{
			get => (Brush)GetValue(CheckedBackgroundProperty);
			set => SetValue(CheckedBackgroundProperty, value);
		}
		public static readonly DependencyProperty CheckedBackgroundProperty =
			DependencyProperty.Register("CheckedBackground", typeof(Brush), typeof(CircularToggleButton), new PropertyMetadata(null));

		public Brush CheckedForeground
		{
			get => (Brush)GetValue(CheckedForegroundProperty);
			set => SetValue(CheckedForegroundProperty, value);
		}
		public static readonly DependencyProperty CheckedForegroundProperty =
			DependencyProperty.Register("CheckedForeground", typeof(Brush), typeof(CircularToggleButton), new PropertyMetadata(null));

		public CircularToggleButton()
		{
			DefaultStyleKey = typeof(CircularToggleButton);
		}
	}
}