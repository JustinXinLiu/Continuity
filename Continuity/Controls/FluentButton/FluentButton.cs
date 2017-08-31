using Continuity.Lights;
using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;

namespace Continuity.Controls
{
    [TemplatePart(Name = PART_RootGrid, Type = typeof(Grid))]
    [TemplatePart(Name = PART_HoverBackground, Type = typeof(Rectangle))]
    public sealed class FluentButton : Button
    {
        #region Fields

        private const string PART_RootGrid = "RootGrid";
        private const string PART_HoverBackground = "HoverBackground";

        private Grid _rootGrid;
        private Rectangle _hoverBackground;

        #endregion

        public FluentButton()
        {
            DefaultStyleKey = typeof(FluentButton);

            Lights.Add(new HoverXamlLight());
            Lights.Add(new AmbientXamlLight());

            Click += OnClick;
            Holding += OnHolding;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void OnHolding(object sender, HoldingRoutedEventArgs e)
        {
        }

        #region Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            //_hoverBackground = GetTemplateChild<Rectangle>(PART_HoverBackground);
            //_rootGrid = GetTemplateChild<Grid>(PART_RootGrid);
        }

        #endregion

        #region Event Handlers



        #endregion

        #region Methods

        private T GetTemplateChild<T>(string name, string message = null) where T : DependencyObject
        {
            if (GetTemplateChild(name) is T child)
            {
                return child;
            }

            if (message == null)
            {
                message = $"{name} should not be null! Check the default Generic.xaml.";
            }

            throw new NullReferenceException(message);
        }

        #endregion
    }
}