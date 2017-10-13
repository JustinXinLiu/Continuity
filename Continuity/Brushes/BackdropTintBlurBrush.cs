﻿using System;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Continuity.Lights;
using Microsoft.Graphics.Canvas.Effects;

namespace Continuity.Brushes
{
    public class BackdropTintBlurBrush : XamlCompositionBrushBase
    {
        public Color TintColor
        {
            get => (Color)GetValue(TintColorProperty);
            set => SetValue(TintColorProperty, value);
        }
        public static readonly DependencyProperty TintColorProperty = DependencyProperty.Register(
            "TintColor", typeof(Color), typeof(BackdropTintBlurBrush), new PropertyMetadata(Colors.Silver, (s, e) =>
            {
            }));


        protected override void OnConnected()
        {
             var compositor = Window.Current.Compositor;

            // CompositionCapabilities: Are Effects supported?
            bool usingFallback = !CompositionCapabilities.GetForCurrentView().AreEffectsSupported();
            if (usingFallback)
            {
                // If Effects are not supported, use Fallback Solid Color
                CompositionBrush = compositor.CreateColorBrush(FallbackColor);
                return;
            }

            // Define Effect graph
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

            // Create EffectFactory and EffectBrush
            var effectFactory = compositor.CreateEffectFactory(graphicsEffect, new[] { "Blur.BlurAmount" });
            var effectBrush = effectFactory.CreateBrush();

            // Create BackdropBrush
            var backdrop = compositor.CreateBackdropBrush();
            effectBrush.SetSourceParameter("backdrop", backdrop);

            // Trivial looping animation to demonstrate animated effects
            var duration = TimeSpan.FromSeconds(5);
            var blurAnimation = compositor.CreateScalarKeyFrameAnimation();
            blurAnimation.InsertKeyFrame(0, 0, compositor.CreateLinearEasingFunction());
            blurAnimation.InsertKeyFrame(0.5f, 24f, compositor.CreateLinearEasingFunction());
            blurAnimation.InsertKeyFrame(1, 0, compositor.CreateLinearEasingFunction());
            blurAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
            blurAnimation.Duration = duration;
            effectBrush.Properties.StartAnimation("Blur.BlurAmount", blurAnimation);

            // Set EffectBrush to paint Xaml UIElement
            CompositionBrush = effectBrush;
        }

        protected override void OnDisconnected()
        {
            // Dispose CompositionBrushes if XamlCompBrushBase is removed from tree
            if (CompositionBrush != null)
            {
                CompositionBrush.Dispose();
                CompositionBrush = null;
            }
        }
    }
}