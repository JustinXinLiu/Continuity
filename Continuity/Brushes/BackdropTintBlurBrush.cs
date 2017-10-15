using System;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Continuity.Extensions;
using Continuity.Lights;
using Microsoft.Graphics.Canvas.Effects;

namespace Continuity.Brushes
{
    public class BackdropTintBlurBrush : XamlCompositionBrushBase
    {
        #region Properties

        public Color TintColor
        {
            get => (Color)GetValue(TintColorProperty);
            set => SetValue(TintColorProperty, value);
        }
        public static readonly DependencyProperty TintColorProperty = DependencyProperty.Register(
            "TintColor", typeof(Color), typeof(BackdropTintBlurBrush), new PropertyMetadata(Colors.GhostWhite));

        public double Duration
        {
            get => (double)GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(
            "Duration", typeof(double), typeof(BackdropTintBlurBrush), new PropertyMetadata(1000d));

        public double BlurAmount
        {
            get => (double)GetValue(BlurAmountProperty);
            set => SetValue(BlurAmountProperty, value);
        }
        public static readonly DependencyProperty BlurAmountProperty = DependencyProperty.Register(
            "BlurAmount", typeof(double), typeof(BackdropTintBlurBrush), new PropertyMetadata(24d));

        #endregion

        #region Overrides

        protected override void OnConnected()
        {
             var compositor = Window.Current.Compositor;

            // CompositionCapabilities: Are Effects supported?
            bool usingFallback = !CompositionCapabilities.GetForCurrentView().AreEffectsSupported();
            if (usingFallback)
            {
                // If Effects are not supported, use Fallback Solid Color.
                CompositionBrush = compositor.CreateColorBrush(FallbackColor);
                return;
            }

            // Define Effect graph.
            var graphicsEffect = new BlendEffect
            {
                Mode = BlendEffectMode.LinearBurn,
                Background = new ColorSourceEffect
                {
                    Name = "Tint",
                    Color = TintColor
                },
                Foreground = new GaussianBlurEffect
                {
                    Name = "Blur",
                    Source = new CompositionEffectSourceParameter("Backdrop"),
                    BlurAmount = 0,
                    BorderMode = EffectBorderMode.Hard
                }
            };

            // Create EffectFactory and EffectBrush.
            var effectFactory = compositor.CreateEffectFactory(graphicsEffect, new[] { "Blur.BlurAmount" });
            var effectBrush = effectFactory.CreateBrush();

            // Create BackdropBrush.
            var backdrop = compositor.CreateBackdropBrush();
            effectBrush.SetSourceParameter("backdrop", backdrop);

            var blurAnimation = compositor.CreateScalarKeyFrameAnimation();
            blurAnimation.InsertKeyFrame(0.0f, 0.0f);
            blurAnimation.InsertKeyFrame(1.0f, BlurAmount.ToFloat(), compositor.CreateLinearEasingFunction());
            blurAnimation.Duration = TimeSpan.FromMilliseconds(Duration);
            effectBrush.Properties.StartAnimation("Blur.BlurAmount", blurAnimation);

            // Set EffectBrush to paint Xaml UIElement.
            CompositionBrush = effectBrush;
        }

        protected override void OnDisconnected()
        {
            // Dispose CompositionBrushes if XamlCompBrushBase is removed from tree.
            CompositionBrush?.Dispose();
            CompositionBrush = null;
        }

        #endregion
    }
}