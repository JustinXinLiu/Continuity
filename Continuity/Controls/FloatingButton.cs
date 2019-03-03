using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using System.Numerics;
using Windows.Devices.Input;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Continuity.Extensions;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Continuity.Controls
{
    [TemplatePart(Name = PART_ContentPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_PointerOverContentPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_ShadowHost, Type = typeof(Shape))]
    public sealed class FloatingButton : Button
    {
        private const string PART_ContentPresenter = "PART_ContentPresenter";
        private const string PART_PointerOverContentPresenter = "PART_PointerOverContentPresenter";
        private const string PART_ShadowHost = "PART_ShadowHost";

        private readonly Color _shadowColor = "#FF72C30E".ToColor();

        private ContentPresenter _pointerOverContent;
        private Shape _shadowHost;

        private Visual _visual;
        private Visual _pointerOverContentVisual;
        private CompositionPropertySet _pointerOverContentHeight;

        private DropShadow _backgroundShadow;

        private ScalarKeyFrameAnimation _showPointerOverPanelAnimation;
        private ScalarKeyFrameAnimation _showTouchOverPanelAnimation;
        private ScalarKeyFrameAnimation _hidePointerOverPanelAnimation;

        private Compositor Compositor => Window.Current.Compositor;

        public Brush PointerOverForeground
        {
            get => (Brush)GetValue(PointerOverForegroundProperty);
            set => SetValue(PointerOverForegroundProperty, value);
        }
        public static readonly DependencyProperty PointerOverForegroundProperty =
            DependencyProperty.Register("PointerOverForeground", typeof(Brush), typeof(FloatingButton), new PropertyMetadata(null));

        public Brush PointerOverBackground
        {
            get => (Brush)GetValue(PointerOverBackgroundProperty);
            set => SetValue(PointerOverBackgroundProperty, value);
        }
        public static readonly DependencyProperty PointerOverBackgroundProperty =
            DependencyProperty.Register("PointerOverBackground", typeof(Brush), typeof(FloatingButton), new PropertyMetadata(null));

        public object PointerOverContent
        {
            get => GetValue(PointerOverContentProperty);
            set => SetValue(PointerOverContentProperty, value);
        }
        public static readonly DependencyProperty PointerOverContentProperty =
            DependencyProperty.Register("PointerOverContent", typeof(object), typeof(FloatingButton), new PropertyMetadata(null));

        public DataTemplate PointerOverContentTemplate
        {
            get => (DataTemplate)GetValue(PointerOverContentTemplateProperty);
            set => SetValue(PointerOverContentTemplateProperty, value);
        }
        public static readonly DependencyProperty PointerOverContentTemplateProperty =
            DependencyProperty.Register("PointerOverContentTemplate", typeof(DataTemplate), typeof(FloatingButton), new PropertyMetadata(null));

        public double ShadowRadius
        {
            get => (double)GetValue(ShadowRadiusProperty);
            set => SetValue(ShadowRadiusProperty, value);
        }
        public static readonly DependencyProperty ShadowRadiusProperty =
            DependencyProperty.Register("ShadowRadius", typeof(double), typeof(FloatingButton), new PropertyMetadata(32.0d));

        public FloatingButton()
        {
            DefaultStyleKey = typeof(FloatingButton);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _pointerOverContent = GetTemplateChild<ContentPresenter>(PART_PointerOverContentPresenter);
            _shadowHost = GetTemplateChild<Shape>(PART_ShadowHost);

            _visual = VisualExtensions.GetVisual(this);
            _pointerOverContentVisual = VisualExtensions.GetVisual(_pointerOverContent);

            _backgroundShadow = CreateDropShadow(0.4f, new Vector3(0, 8.0f, 0), 24.0f, _shadowColor);
            _backgroundShadow.Mask = _shadowHost.GetAlphaMask();
            var shadowVisual = Compositor.CreateSpriteVisual();
            shadowVisual.Shadow = _backgroundShadow;
            shadowVisual.RelativeSizeAdjustment = Vector2.One;
            ElementCompositionPreview.SetElementChildVisual(_shadowHost, shadowVisual);

            _visual.RotationAxis = new Vector3(1.0f, 0.0f, 0.0f);

            _showPointerOverPanelAnimation = CreateShowClipAnimation();
            _hidePointerOverPanelAnimation = CreateHideClipAnimation();
            _showTouchOverPanelAnimation = CreateShowClipAnimation(25);
            ScalarKeyFrameAnimation CreateShowClipAnimation(int duration = 400)
            {
                var clipAnimation = Compositor.CreateScalarKeyFrameAnimation();
                clipAnimation.InsertKeyFrame(1.0f, 0);
                clipAnimation.Duration = TimeSpan.FromMilliseconds(duration);
                return clipAnimation;
            }
            ScalarKeyFrameAnimation CreateHideClipAnimation()
            {
                _pointerOverContentHeight = Compositor.CreatePropertySet();

                var clipAnimation = Compositor.CreateScalarKeyFrameAnimation();
                clipAnimation.InsertExpressionKeyFrame(1.0f, _pointerOverContentHeight.GetReference().GetScalarProperty("Value"));
                clipAnimation.Duration = TimeSpan.FromMilliseconds(300);

                return clipAnimation;
            }

            SizeChanged += OnSizeChanged;
            PointerEntered += OnPointerEntered;
            AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
            AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
            PointerExited += OnPointerExited;
            PointerCanceled += OnPointerExited;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Equals(e.NewSize)) return;

            var pointerOverPanelHeight = (float)_pointerOverContent.ActualHeight;
            _pointerOverContentHeight.InsertScalar("Value", pointerOverPanelHeight);

            var bottomClip = Compositor.CreateInsetClip(0, 0, 0, pointerOverPanelHeight);
            VisualExtensions.GetVisual(_pointerOverContent).Clip = bottomClip;

            _visual.CenterPoint = new Vector3(RenderSize.ToVector2() / 2, 0.0f);
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                _pointerOverContentVisual.Clip.StartAnimation(nameof(InsetClip.BottomInset), _showTouchOverPanelAnimation);
            }
            else
            {
                _pointerOverContentVisual.Clip.StartAnimation(nameof(InsetClip.BottomInset), _showPointerOverPanelAnimation);
            }

            _visual.StartScaleAnimation(to: new Vector2(1.02f), duration: 400);
            _backgroundShadow.StartShadowBlurRadiusAnimation(_shadowColor, new Vector3(0, 12.0f, 0), toShadowOpacity: 0.5f, toBlurRadius: 36.0f, duration: 600);
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _visual.StartScaleAnimation(to: new Vector2(0.98f), duration: 300);
            _backgroundShadow.StartShadowBlurRadiusAnimation(_shadowColor, new Vector3(0, 4.0f, 0), toShadowOpacity: 0.6f, toBlurRadius: 16.0f, duration: 300);
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _visual.StartScaleAnimation(to: new Vector2(1.02f), duration: 200);
            _backgroundShadow.StartShadowBlurRadiusAnimation(_shadowColor, new Vector3(0, 12.0f, 0), toShadowOpacity: 0.5f, toBlurRadius: 36.0f, duration: 300);
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            _pointerOverContentVisual.Clip.StartAnimation(nameof(InsetClip.BottomInset), _hidePointerOverPanelAnimation);

            _visual.StartScaleAnimation(to: Vector2.One, duration: 300);
            _backgroundShadow.StartShadowBlurRadiusAnimation(_shadowColor, new Vector3(0, 8.0f, 0), toShadowOpacity: 0.4f, toBlurRadius: 24.0f, duration: 700, delay: 100);
        }

        private DropShadow CreateDropShadow(float opacity = 0.0f, Vector3 offset = default(Vector3),
            float blurRadius = 0.0f, Color? color = null)
        {
            var shadow = Compositor.CreateDropShadow();

            shadow.Opacity = opacity;
            shadow.Offset = offset;
            shadow.BlurRadius = blurRadius;

            if (color.HasValue)
            {
                shadow.Color = color.Value;
            }

            return shadow;
        }

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
    }
}