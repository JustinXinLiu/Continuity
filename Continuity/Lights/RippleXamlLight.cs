using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Continuity.Lights
{
    public class RippleXamlLight : XamlLight
    {
        #region Fields

        private static readonly string Id = typeof(RippleXamlLight).FullName;

        #endregion

        #region Properties

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color", typeof(Color), typeof(RippleXamlLight), new PropertyMetadata(Colors.White, (s, e) =>
            {
                var self = (RippleXamlLight)s;
                var newColor = (Color) e.NewValue;

                if (self.CompositionLight is SpotLight spotLight)
                {
                    //spotLight.Color = newColor;
                }
            }));

        #endregion

        #region Overrides

        protected override void OnConnected(UIElement newElement)
        {
            var compositor = Window.Current.Compositor;
            var spotLight = compositor.CreateSpotLight();
            //spotLight.InnerConeColor = 

            //CompositionLight = spotLight;

            AddTargetElement(GetId(), newElement);
        }

        protected override void OnDisconnected(UIElement oldElement)
        {
            RemoveTargetElement(GetId(), oldElement);
            CompositionLight.Dispose();
        }

        protected override string GetId() => Id;

        #endregion

        #region Methods

        //private void ResetLightOffsetAnimation() =>
        //    CompositionLight?.StartAnimation("Offset", _lightRestingOffsetAnimation);

        #endregion
    }
}
