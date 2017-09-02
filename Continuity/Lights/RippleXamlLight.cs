using System;
using System.Diagnostics;
using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Continuity.Lights
{
    public class RippleXamlLight : XamlLight
    {
        #region Fields

        private static readonly string Id = typeof(RippleXamlLight).FullName;

        private Compositor _compositor;

        private const float OffsetZRatio = 0.6f;
        private float _hoverOffsetZ;
        private Vector3KeyFrameAnimation _lightOffsetAnimation;

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
                    spotLight.InnerConeColor = newColor;
                    spotLight.OuterConeColor = newColor;
                }
            }));

        #endregion

        #region Overrides

        protected override void OnConnected(UIElement newElement)
        {
            _compositor = Window.Current.Compositor;
            var spotLight = _compositor.CreateSpotLight();
            spotLight.InnerConeAngleInDegrees = 50f;
            spotLight.InnerConeColor = Colors.FloralWhite;
            spotLight.OuterConeAngleInDegrees = 0f;
            spotLight.ConstantAttenuation = 1f;
            spotLight.LinearAttenuation = 0.253f;
            spotLight.QuadraticAttenuation = 0.58f;

            CompositionLight = spotLight;

            _lightOffsetAnimation = CreateLightOffsetAnimation();

            SubscribeToPointerEvents();

            AddTargetElement(GetId(), newElement);

            void SubscribeToPointerEvents()
            {
                if (newElement is FrameworkElement element)
                {
                    element.SizeChanged += OnElementSizeChanged;
                }
                if (newElement is ButtonBase button)
                {
                    button.Click += OnElementClick;
                }
                else
                {
                    newElement.Tapped += OnElementTapped;
                }
            }
        }

        protected override void OnDisconnected(UIElement oldElement)
        {
            UnsubscribeFromPointerEvents();

            RemoveTargetElement(GetId(), oldElement);
            CompositionLight.Dispose();

            _lightOffsetAnimation.Dispose();

            void UnsubscribeFromPointerEvents()
            {
                if (oldElement is FrameworkElement element)
                {
                    element.SizeChanged -= OnElementSizeChanged;
                }
                if (oldElement is ButtonBase button)
                {
                    button.Click -= OnElementClick;
                }
                else
                {
                    oldElement.Tapped -= OnElementTapped;
                }
            }
        }

        protected override string GetId() => Id;

        #endregion

        #region Event Handlers

        private void OnElementSizeChanged(object sender, SizeChangedEventArgs e) =>
            _hoverOffsetZ = CalculateHoverOffsetZOnRenderSize((FrameworkElement)sender);

        private void OnElementClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Clicked");
        }

        private void OnElementTapped(object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine("Tapped");
        }

        #endregion

        #region Methods

        private Vector3KeyFrameAnimation CreateLightOffsetAnimation()
        {
            var easing = _compositor.CreateCubicBezierEasingFunction(new Vector2(0.3f, 0.7f), new Vector2(0.9f, 0.5f));

            var offsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
            //offsetAnimation.InsertKeyFrame(1.0f, RestingOffset, easing);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(800);

            return offsetAnimation;
        }

        private void StartLightOffsetAnimation() =>
            CompositionLight?.StartAnimation("Offset", _lightOffsetAnimation);

        private float CalculateHoverOffsetZOnRenderSize(UIElement element)
        {
            var desiredSize = element.RenderSize.ToVector2();
            return Math.Max(desiredSize.X, desiredSize.Y) * OffsetZRatio;
        }

        #endregion
    }
}
