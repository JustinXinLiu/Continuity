using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Continuity.Controls
{
    public sealed class TabItem : ContentControl
    {
        public TabItem()
        {
            DefaultStyleKey = typeof(TabItem);
        }

        #region Properties

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(TabItem), new PropertyMetadata(null, (s, dp) =>
            {
                var value = dp.NewValue;

                if (!(value is string)) return;

                var self = (TabItem)s;
                self.Header = self.Header.ToString().ToUpperInvariant();
            }));

        public Style HeaderIconStyle
        {
            get { return (Style)GetValue(HeaderIconStyleProperty); }
            set { SetValue(HeaderIconStyleProperty, value); }
        }

        public static readonly DependencyProperty HeaderIconStyleProperty =
            DependencyProperty.Register("HeaderIconStyle", typeof(Style), typeof(TabItem), new PropertyMetadata(null));

        #endregion

        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
