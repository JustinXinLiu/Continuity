using Continuity.Extensions;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;

namespace Continuity.Controls
{
    [TemplatePart(Name = ShadowShapeName, Type = typeof(Rectangle))]
    public sealed class Shadow : ContentControl
    {
        #region Fields

        private const string ShadowShapeName = "ShadowShape";

        private Rectangle _shadowShape;

        private readonly Compositor _compositor;
        private readonly DropShadow _dropShadow;
        private readonly SpriteVisual _shadowVisual;

        #endregion

        public Shadow()
        {
            DefaultStyleKey = typeof(Shadow);

            if (DesignMode.DesignModeEnabled) return;

            _compositor = this.Visual().Compositor;

            _shadowVisual = _compositor.CreateSpriteVisual();
            _dropShadow = _compositor.CreateDropShadow();
            _shadowVisual.Shadow = _dropShadow;
        }

        #region Properties

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(Shadow), new PropertyMetadata(Colors.Black, OnColorChanged));

        public double ShadowOpacity
        {
            get { return (double)GetValue(ShadowOpacityProperty); }
            set { SetValue(ShadowOpacityProperty, value); }
        }
        public static readonly DependencyProperty ShadowOpacityProperty =
            DependencyProperty.Register("ShadowOpacity", typeof(double), typeof(Shadow), new PropertyMetadata(1.0d, OnShadowOpacityChanged));

        public double BlurRadius
        {
            get { return (double)GetValue(BlurRadiusProperty); }
            set { SetValue(BlurRadiusProperty, value); }
        }
        public static readonly DependencyProperty BlurRadiusProperty =
             DependencyProperty.Register("BlurRadius", typeof(double), typeof(Shadow), new PropertyMetadata(4.0d, OnBlurRadiusChanged));

        public double RadiusX
        {
            get { return (double)GetValue(RadiusXProperty); }
            set { SetValue(RadiusXProperty, value); }
        }
        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register("RadiusX", typeof(double), typeof(Shadow), new PropertyMetadata(0.0d));

        public double RadiusY
        {
            get { return (double)GetValue(RadiusYProperty); }
            set { SetValue(RadiusYProperty, value); }
        }
        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register("RadiusY", typeof(double), typeof(Shadow), new PropertyMetadata(0.0d));

        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }
        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register("OffsetX", typeof(double), typeof(Shadow), new PropertyMetadata(0.0d, OnOffsetXChanged));

        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(double), typeof(Shadow), new PropertyMetadata(0.0d, OnOffsetYChanged));

        public double OffsetZ
        {
            get { return (double)GetValue(OffsetZProperty); }
            set { SetValue(OffsetZProperty, value); }
        }
        public static readonly DependencyProperty OffsetZProperty =
            DependencyProperty.Register("OffsetZ", typeof(double), typeof(Shadow), new PropertyMetadata(0.0d, OnOffsetZChanged));

        public bool AllowHoverEffect
        {
            get { return (bool)GetValue(AllowHoverEffectProperty); }
            set { SetValue(AllowHoverEffectProperty, value); }
        }
        public static readonly DependencyProperty AllowHoverEffectProperty =
            DependencyProperty.Register("AllowHoverEffect", typeof(bool), typeof(Shadow), new PropertyMetadata(true));

        #endregion

        #region Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (DesignMode.DesignModeEnabled) return;

            _shadowShape = GetTemplateChild<Rectangle>(ShadowShapeName);
            _shadowShape.SetChildVisual(_shadowVisual);

            Loaded += (s, e) =>
            {
                UpdateShadowMask();
                UpdateShadowSize();
                UpdateRadius();
            };

            SizeChanged += (s, e) => UpdateShadowSize();

            PointerEntered += OnPointerEntered;
            PointerExited += OnPointerExited;
            PointerCaptureLost += OnPointerExited;
            PointerCanceled += OnPointerExited;
        }

        #endregion

        #region Event Handlers

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!AllowHoverEffect) return;

            AnimateShadowBlurRadius(BlurRadius * 3.0d);
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!AllowHoverEffect) return;

            AnimateShadowBlurRadius(BlurRadius);
        }

        #endregion

        #region Property Changed Handlers

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (Shadow)d;
            self._dropShadow.Color = (Color)e.NewValue;
        }

        private static void OnBlurRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (Shadow)d;
            self._dropShadow.BlurRadius = float.Parse(e.NewValue.ToString());
        }

        private static void OnShadowOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (Shadow)d;
            self._dropShadow.Opacity = float.Parse(e.NewValue.ToString());
        }

        private static void OnOffsetXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (Shadow)d;
            // ReSharper disable once ArgumentsStyleOther
            self.UpdateShadowOffset(x: float.Parse(e.NewValue.ToString()));
        }
        private static void OnOffsetYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (Shadow)d;
            self.UpdateShadowOffset(y: float.Parse(e.NewValue.ToString()));
        }

        private static void OnOffsetZChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (Shadow)d;
            self.UpdateShadowOffset(z: float.Parse(e.NewValue.ToString()));
        }

        #endregion

        #region Methods

        private void UpdateShadowMask()
        {
            var mask = _shadowShape.GetAlphaMask();
            _dropShadow.Mask = mask;
        }

        private void UpdateShadowSize()
        {
            var newSize = new Vector2(_shadowShape.ActualWidth.ToFloat(), _shadowShape.ActualHeight.ToFloat());
            _shadowVisual.Size = newSize;
        }

        private void UpdateShadowOffset(float? x = null, float? y = null, float? z = null)
        {
            x = x ?? _dropShadow.Offset.X;
            y = y ?? _dropShadow.Offset.Y;
            z = z ?? _dropShadow.Offset.Z;

            _dropShadow.Offset = new Vector3(x.Value, y.Value, z.Value);
        }

        private void AnimateShadowBlurRadius(double blurRadius, int duration = 350)
        {
            var animation = _compositor.CreateScalarKeyFrameAnimation();
            animation.InsertKeyFrame(1.0f, blurRadius.ToFloat());
            animation.Duration = TimeSpan.FromMilliseconds(duration);
            _dropShadow.StartAnimation("BlurRadius", animation);
        }

        private void UpdateRadius()
        {
            var child = Content as FrameworkElement;
            if (child == null || !RadiusX.Equals(0) || !RadiusY.Equals(0)) return;

            var types = new Dictionary<Type, Action>
            {
                { typeof(Grid), () => RadiusX = RadiusY = ((Grid)child).CornerRadius.TopLeft },
                { typeof(Border), () => RadiusX = RadiusY = ((Border)child).CornerRadius.TopLeft },
                { typeof(RelativePanel), () => RadiusX = RadiusY = ((RelativePanel)child).CornerRadius.TopLeft },
                { typeof(StackPanel), () => RadiusX = RadiusY = ((StackPanel)child).CornerRadius.TopLeft }
            };

            if (types.ContainsKey(child.GetType()))
            {
                types[child.GetType()]();
            }
        }

        // TODO: Update this with C# 7.
        //private void UpdateRadius()
        //{
        //    var child = Content as FrameworkElement;
        //    if (child == null || !RadiusX.Equals(0) || !RadiusY.Equals(0)) return;

        //    switch (child)
        //    {
        //        case Grid g:
        //            RadiusX = RadiusY = g.CornerRadius.TopLeft;
        //            break;
        //        case Border b:
        //            RadiusX = RadiusY = b.CornerRadius.TopLeft;
        //            break;
        //        case RelativePanel r:
        //            RadiusX = RadiusY = r.CornerRadius.TopLeft;
        //            break;
        //        case StackPanel s:
        //            RadiusX = RadiusY = s.CornerRadius.TopLeft;
        //            break;
        //    }
        //}

        public T GetTemplateChild<T>(string name, string message = null) where T : DependencyObject
        {
            var child = GetTemplateChild(name) as T;

            if (child != null)
                return child;

            if (message == null)
            {
                message = $"{name} should not be null! Check the default Generic.xaml.";
            }

            throw new NullReferenceException(message);
        }

        #endregion
    }
}