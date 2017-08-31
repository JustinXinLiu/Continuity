using System;
using System.Diagnostics;
using Continuity.Extensions;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using System.Numerics;
using Windows.UI.Xaml.Controls.Primitives;
using EF = ExpressionBuilder.ExpressionFunctions;
using ExpressionBuilder;

namespace Continuity.Lights
{
    public class HoverXamlLight : XamlLight
    {
        #region Fields

        private static readonly string Id = typeof(HoverXamlLight).FullName;

        private Compositor _compositor;

        private const int IncreaseIntensityDuration = 400;
        private const int DecreaseIntensityDuration = 1200;
        private const float InnerConeIntensity = 0.1f;
        private const float OuterConeIntensity = 0.2f;
        private ScalarKeyFrameAnimation _lightIncreaseInnerConeIntensityAnimation;
        private ScalarKeyFrameAnimation _lightIncreaseOuterConeIntensityAnimation;
        private ScalarKeyFrameAnimation _lightDecreaseInnerConeIntensityAnimation;
        private ScalarKeyFrameAnimation _lightDecreaseOuterConeIntensityAnimation;

        private static readonly Vector3 RestingOffset = new Vector3(0, 0, 400);
        private Vector3KeyFrameAnimation _lightRestingOffsetAnimation;

        private float _hoverOffsetZ;
        private Vector3Node _lightPositionExpressionNode;

        #endregion

        #region Properties

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color", typeof(Color), typeof(HoverXamlLight), new PropertyMetadata(Colors.White, (s, e) =>
            {
                var self = (HoverXamlLight)s;
                var newColor = (Color)e.NewValue;

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
            _hoverOffsetZ = CalculateHoverOffsetZOnDesiredSize(newElement);

            _compositor = Window.Current.Compositor;
            var spotLight = _compositor.CreateSpotLight();
            spotLight.CoordinateSpace = newElement.Visual();
            spotLight.InnerConeColor = spotLight.OuterConeColor = Color;
            spotLight.InnerConeIntensity = spotLight.OuterConeIntensity = 0.0f;
            spotLight.InnerConeAngleInDegrees = 0.0f;
            spotLight.OuterConeAngleInDegrees = 90.0f;

            CompositionLight = spotLight;

            _lightIncreaseInnerConeIntensityAnimation = CreateLightIncreaseInnerConeIntensityAnimation();
            _lightIncreaseOuterConeIntensityAnimation = CreateLightIncreaseOuterConeIntensityAnimation();
            _lightDecreaseInnerConeIntensityAnimation = CreateLightDecreaseInnerConeIntensityAnimation();
            _lightDecreaseOuterConeIntensityAnimation = CreateLightDecreaseOuterConeIntensityAnimation();
            _lightRestingOffsetAnimation = CreateLightRestingOffsetAnimation();
            _lightPositionExpressionNode = CreateLightPositionExpressionNode(newElement);

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
                newElement.PointerEntered += OnElementPointerEntered;
                newElement.PointerExited += OnElementPointerExited;
                newElement.PointerCanceled += OnElementPointerExited;
            }
        }

        protected override void OnDisconnected(UIElement oldElement)
        {
            UnsubscribeFromPointerEvents();

            RemoveTargetElement(GetId(), oldElement);
            CompositionLight.Dispose();

            _lightIncreaseInnerConeIntensityAnimation.Dispose();
            _lightIncreaseOuterConeIntensityAnimation.Dispose();
            _lightDecreaseInnerConeIntensityAnimation.Dispose();
            _lightDecreaseOuterConeIntensityAnimation.Dispose();
            _lightRestingOffsetAnimation.Dispose();
            _lightPositionExpressionNode.Dispose();

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
                oldElement.PointerEntered -= OnElementPointerEntered;
                oldElement.PointerExited -= OnElementPointerExited;
                oldElement.PointerCanceled -= OnElementPointerExited;
            }
        }

        protected override string GetId() => Id;

        #endregion

        #region Event Handlers

        private void OnElementSizeChanged(object sender, SizeChangedEventArgs e) => 
            _hoverOffsetZ = CalculateHoverOffsetZOnRenderSize((FrameworkElement)sender);

        private void OnElementPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Debug.WriteLine("Entered");

            ShowLightAnimation();
            StartLightOffsetAnimation();
        }

        private void OnElementClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Clicked");
        }

        private void OnElementTapped(object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine("Tapped");
        }

        private void OnElementPointerExited(object sender, PointerRoutedEventArgs e)
        {
            Debug.WriteLine("Exited");
            
            HideLightAnimation();
        }

        #endregion

        #region Methods

        private ScalarKeyFrameAnimation CreateLightIncreaseInnerConeIntensityAnimation()
        {
            var intensityAnimation = _compositor.CreateScalarKeyFrameAnimation();
            intensityAnimation.InsertKeyFrame(1.0f, InnerConeIntensity);
            intensityAnimation.Duration = TimeSpan.FromMilliseconds(IncreaseIntensityDuration);

            return intensityAnimation;
        }

        private ScalarKeyFrameAnimation CreateLightIncreaseOuterConeIntensityAnimation()
        {
            var intensityAnimation = _compositor.CreateScalarKeyFrameAnimation();
            intensityAnimation.InsertKeyFrame(1.0f, OuterConeIntensity);
            intensityAnimation.Duration = TimeSpan.FromMilliseconds(IncreaseIntensityDuration);

            return intensityAnimation;
        }

        private ScalarKeyFrameAnimation CreateLightDecreaseInnerConeIntensityAnimation()
        {
            var intensityAnimation = _compositor.CreateScalarKeyFrameAnimation();
            intensityAnimation.InsertKeyFrame(1.0f, 0.0f);
            intensityAnimation.Duration = TimeSpan.FromMilliseconds(DecreaseIntensityDuration);

            return intensityAnimation;
        }

        private ScalarKeyFrameAnimation CreateLightDecreaseOuterConeIntensityAnimation()
        {
            var intensityAnimation = _compositor.CreateScalarKeyFrameAnimation();
            intensityAnimation.InsertKeyFrame(1.0f, 0.0f);
            intensityAnimation.Duration = TimeSpan.FromMilliseconds(DecreaseIntensityDuration);

            return intensityAnimation;
        }

        private Vector3KeyFrameAnimation CreateLightRestingOffsetAnimation()
        {
            var easing = _compositor.CreateCubicBezierEasingFunction(new Vector2(0.3f, 0.7f), new Vector2(0.9f, 0.5f));

            var offsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1.0f, RestingOffset, easing);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(800);

            return offsetAnimation;
        }

        private Vector3Node CreateLightPositionExpressionNode(UIElement element)
        {
            var hoverPositionProperties = element.GetPointerPositionProperties();

            var hoverPosition = hoverPositionProperties.GetSpecializedReference<PointerPositionPropertySetReferenceNode>().Position;
            var positionExpressionNode = EF.Vector3(hoverPosition.X, hoverPosition.Y, _hoverOffsetZ);

            return positionExpressionNode;
        }

        private void ShowLightAnimation()
        {
            CompositionLight?.StartAnimation("InnerConeIntensity", _lightIncreaseInnerConeIntensityAnimation);
            CompositionLight?.StartAnimation("OuterConeIntensity", _lightIncreaseOuterConeIntensityAnimation);
        }

        private void HideLightAnimation()
        {
            CompositionLight?.StartAnimation("InnerConeIntensity", _lightDecreaseInnerConeIntensityAnimation);
            CompositionLight?.StartAnimation("OuterConeIntensity", _lightDecreaseOuterConeIntensityAnimation);
        }

        private void StartLightOffsetAnimation() =>
            CompositionLight?.StartAnimation("Offset", _lightPositionExpressionNode);

        private float CalculateHoverOffsetZOnDesiredSize(UIElement element)
        {
            var desiredSize = element.GetDesiredSize();
            return Math.Max(desiredSize.X, desiredSize.Y) / 2;
        }

        private float CalculateHoverOffsetZOnRenderSize(UIElement element)
        {
            var desiredSize = element.RenderSize.ToVector2();
            return Math.Max(desiredSize.X, desiredSize.Y) / 2;
        }

        #endregion
    }
}
