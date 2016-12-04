using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Continuity.Controls.Tab
{
    public sealed class TabHeaderItem : RadioButton
    {
        public TabHeaderItem()
        {
            DefaultStyleKey = typeof(TabHeaderItem);
        }

        #region Properties

        public Style HeaderIconStyle
        {
            get { return (Style)GetValue(HeaderIconStyleProperty); }
            set { SetValue(HeaderIconStyleProperty, value); }
        }

        public static readonly DependencyProperty HeaderIconStyleProperty =
            DependencyProperty.Register("HeaderIconStyle", typeof(Style), typeof(TabHeaderItem), new PropertyMetadata(null));

        #endregion
    }
}
