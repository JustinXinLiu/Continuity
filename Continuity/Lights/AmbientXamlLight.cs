using Continuity.Extensions;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Continuity.Lights
{
    public class AmbientXamlLight : XamlLight
    {
        #region Fields

        private static readonly string Id = typeof(AmbientXamlLight).FullName;

        #endregion

        #region Properties

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color", typeof(Color), typeof(AmbientXamlLight), new PropertyMetadata(Colors.White, (s, e) =>
            {
                var self = (AmbientXamlLight)s;
                var newColor = (Color) e.NewValue;

                if (self.CompositionLight is AmbientLight ambientLight)
                {
                    ambientLight.Color = newColor;
                }
            }));

        public double Intensity
        {
            get => (double)GetValue(IntensityProperty);
            set => SetValue(IntensityProperty, value);
        }
        public static readonly DependencyProperty IntensityProperty = DependencyProperty.Register(
            "Intensity", typeof(double), typeof(AmbientXamlLight), new PropertyMetadata(1.0d, (s, e) =>
            {
                var self = (AmbientXamlLight)s;
                var newIntensity = (float)e.NewValue;

                if (self.CompositionLight is AmbientLight ambientLight)
                {
                    ambientLight.Intensity = newIntensity;
                }
            }));

        #endregion

        #region Overrides

        protected override void OnConnected(UIElement newElement)
        {
            var compositor = Window.Current.Compositor;
            var ambientLight = compositor.CreateAmbientLight();
            ambientLight.Color = Color;
            ambientLight.Intensity = Intensity.ToFloat();

            CompositionLight = ambientLight;

            AddTargetElement(GetId(), newElement);
        }

        protected override void OnDisconnected(UIElement oldElement)
        {
            RemoveTargetElement(GetId(), oldElement);
            CompositionLight.Dispose();
        }

        protected override string GetId() => Id;

        #endregion
    }
}
