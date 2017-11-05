using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace Continuity.Controls
{
	public sealed class CircularToggleButton : ToggleButton
	{
		public object CheckedContent
		{
			get { return (object)GetValue(CheckedContentProperty); }
			set { SetValue(CheckedContentProperty, value); }
		}
		public static readonly DependencyProperty CheckedContentProperty =
			DependencyProperty.Register("CheckedContent", typeof(object), typeof(CircularToggleButton), new PropertyMetadata(null));

		public DataTemplate CheckedContentTemplate
		{
			get { return (DataTemplate)GetValue(CheckedContentTemplateProperty); }
			set { SetValue(CheckedContentTemplateProperty, value); }
		}
		public static readonly DependencyProperty CheckedContentTemplateProperty =
			DependencyProperty.Register("CheckedContentTemplate", typeof(DataTemplate), typeof(CircularToggleButton), new PropertyMetadata(null));

		public CornerRadius CornerRadius
		{
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}
		public static readonly DependencyProperty CornerRadiusProperty =
			DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(CircularToggleButton), new PropertyMetadata(new CornerRadius(12, 0, 0, 12)));

		public CornerRadius CheckedCornerRadius
		{
			get { return (CornerRadius)GetValue(CheckedCornerRadiusProperty); }
			set { SetValue(CheckedCornerRadiusProperty, value); }
		}
		public static readonly DependencyProperty CheckedCornerRadiusProperty =
			DependencyProperty.Register("CheckedCornerRadius", typeof(CornerRadius), typeof(CircularToggleButton), new PropertyMetadata(new CornerRadius(16)));

		public CircularToggleButton()
		{
			DefaultStyleKey = typeof(CircularToggleButton);
		}
	}
}