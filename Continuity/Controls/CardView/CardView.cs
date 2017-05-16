using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Continuity.Controls
{
    public sealed class CardView : ListBox
    {
        public CardView()
        {
            DefaultStyleKey = typeof(CardView);
        }

        #region Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CardViewItem();
        }

        #endregion
    }
}