using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Continuity.Controls
{
    public partial class AdaptiveGridViewItem 
    {
        public static void SetIsTitle(DependencyObject element, bool value)
        {
            element.SetValue(IsTitleProperty, value);
        }
        public static bool GetIsTitle(DependencyObject element)
        {
            return (bool) element.GetValue(IsTitleProperty);
        }
        public static readonly DependencyProperty IsTitleProperty = DependencyProperty.RegisterAttached(
            "IsTitle", typeof(bool), typeof(AdaptiveGridViewItem), new PropertyMetadata(default(bool)));
    }
}