using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using System.Numerics;
using Windows.Devices.Input;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Continuity.Extensions;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Continuity.Controls
{
    [TemplatePart(Name = PART_FrontPanel, Type = typeof(Grid))]
    [TemplatePart(Name = PART_BackPanel, Type = typeof(Border))]
    [TemplatePart(Name = PART_ContentPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_CheckedContentPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_PointerOverContentPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_ShadowHost, Type = typeof(Rectangle))]
    public sealed class FlipToggleButton : ToggleButton
    {
        private const string PART_FrontPanel = "PART_FrontPanel";
        private const string PART_BackPanel = "PART_BackPanel";
        private const string PART_ContentPresenter = "PART_ContentPresenter";
        private const string PART_PointerOverContentPresenter = "PART_PointerOverContentPresenter";
        private const string PART_CheckedContentPresenter = "PART_CheckedContentPresenter";
        private const string PART_ShadowHost = "PART_ShadowHost";

        private readonly Color _shadowColor = "#FF905F52".ToColor();

        private Grid _frontPanel;
        private Border _backPanel;
        private ContentPresenter _pointerOverContent;
        private Rectangle _shadowHost;

        private Visual _visual;
        private Visual _frontPanelVisual;
        private Visual _backPanelVisual;
        private Visual _pointerOverContentVisual;
        private CompositionPropertySet _pointerOverContentHeight;
        private LinearEasingFunction _linearEasing;

        private DropShadow _backgroundShadow;

        private ScalarKeyFrameAnimation _showPointerOverPanelAnimation;
        private ScalarKeyFrameAnimation _showTouchOverPanelAnimation;
        private ScalarKeyFrameAnimation _hidePointerOverPanelAnimation;

        private ScalarKeyFrameAnimation _showPanelAnimation;
        private ScalarKeyFrameAnimation _hidePanelAnimation;
        private ScalarKeyFrameAnimation _rotateToBackAnimation;
        private ScalarKeyFrameAnimation _rotateToFrontAnimation;

        private Compositor Compositor => Window.Current.Compositor;

        public Brush PointerOverForeground
        {
            get => (Brush)GetValue(PointerOverForegroundProperty);
            set => SetValue(PointerOverForegroundProperty, value);
        }
        public static readonly DependencyProperty PointerOverForegroundProperty =
            DependencyProperty.Register("PointerOverForeground", typeof(Brush), typeof(FlipToggleButton), new PropertyMetadata(null));

        public Brush PointerOverBackground
        {
            get => (Brush)GetValue(PointerOverBackgroundProperty);
            set => SetValue(PointerOverBackgroundProperty, value);
        }
        public static readonly DependencyProperty PointerOverBackgroundProperty =
            DependencyProperty.Register("PointerOverBackground", typeof(Brush), typeof(FlipToggleButton), new PropertyMetadata(null));

        public object PointerOverContent
        {
            get => GetValue(PointerOverContentProperty);
            set => SetValue(PointerOverContentProperty, value);
        }
        public static readonly DependencyProperty PointerOverContentProperty =
            DependencyProperty.Register("PointerOverContent", typeof(object), typeof(FlipToggleButton), new PropertyMetadata(null));

        public DataTemplate PointerOverContentTemplate
        {
            get => (DataTemplate)GetValue(PointerOverContentTemplateProperty);
            set => SetValue(PointerOverContentTemplateProperty, value);
        }
        public static readonly DependencyProperty PointerOverContentTemplateProperty =
            DependencyProperty.Register("PointerOverContentTemplate", typeof(DataTemplate), typeof(FlipToggleButton), new PropertyMetadata(null));

        public Brush CheckedForeground
        {
            get => (Brush)GetValue(CheckedForegroundProperty);
            set => SetValue(CheckedForegroundProperty, value);
        }
        public static readonly DependencyProperty CheckedForegroundProperty =
            DependencyProperty.Register("CheckedForeground", typeof(Brush), typeof(FlipToggleButton), new PropertyMetadata(null));

        public Brush CheckedBackground
        {
            get => (Brush)GetValue(CheckedBackgroundProperty);
            set => SetValue(CheckedBackgroundProperty, value);
        }
        public static readonly DependencyProperty CheckedBackgroundProperty =
            DependencyProperty.Register("CheckedBackground", typeof(Brush), typeof(FlipToggleButton), new PropertyMetadata(null));

        public object CheckedContent
        {
            get => GetValue(CheckedContentProperty);
            set => SetValue(CheckedContentProperty, value);
        }
        public static readonly DependencyProperty CheckedContentProperty =
            DependencyProperty.Register("CheckedContent", typeof(object), typeof(FlipToggleButton), new PropertyMetadata(null));

        public DataTemplate CheckedContentTemplate
        {
            get => (DataTemplate)GetValue(CheckedContentTemplateProperty);
            set => SetValue(CheckedContentTemplateProperty, value);
        }
        public static readonly DependencyProperty CheckedContentTemplateProperty =
            DependencyProperty.Register("CheckedContentTemplate", typeof(DataTemplate), typeof(FlipToggleButton), new PropertyMetadata(null));

        public FlipToggleButton()
        {
            DefaultStyleKey = typeof(FlipToggleButton);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _linearEasing = Compositor.CreateLinearEasingFunction();

            _pointerOverContent = GetTemplateChild<ContentPresenter>(PART_PointerOverContentPresenter);
            _frontPanel = GetTemplateChild<Grid>(PART_FrontPanel);
            _backPanel = GetTemplateChild<Border>(PART_BackPanel);
            _shadowHost = GetTemplateChild<Rectangle>(PART_ShadowHost);

            _visual = VisualExtensions.GetVisual(this);
            _pointerOverContentVisual = VisualExtensions.GetVisual(_pointerOverContent);
            _frontPanelVisual = VisualExtensions.GetVisual(_frontPanel);
            _backPanelVisual = VisualExtensions.GetVisual(_backPanel);
            _backPanelVisual.Opacity = 0;

            _backgroundShadow = CreateDropShadow(0, Vector3.Zero, 0, _shadowColor);
            var shadowVisual = Compositor.CreateSpriteVisual();
            shadowVisual.Shadow = _backgroundShadow;
            shadowVisual.RelativeSizeAdjustment = Vector2.One;
            ElementCompositionPreview.SetElementChildVisual(_shadowHost, shadowVisual);

            _visual.RotationAxis = new Vector3(1.0f, 0.0f, 0.0f);
            _backPanelVisual.RotationAxis = new Vector3(1.0f, 0.0f, 0.0f);
            _backPanelVisual.RotationAngleInDegrees = 180.0f;
            _rotateToBackAnimation = CreateRotateToBackAnimation();
            _rotateToFrontAnimation = CreateRotateToFrontAnimation();
            _showPanelAnimation = CreateShowPanelAnimation();
            _hidePanelAnimation = CreateHidePanelAnimation();
            ScalarKeyFrameAnimation CreateRotateToBackAnimation()
            {
                var rotationAnimation = Compositor.CreateScalarKeyFrameAnimation();
                rotationAnimation.InsertKeyFrame(1.0f, 180.0f, _linearEasing);
                rotationAnimation.Duration = TimeSpan.FromMilliseconds(300);

                return rotationAnimation;
            }
            ScalarKeyFrameAnimation CreateRotateToFrontAnimation()
            {
                var rotationAnimation = Compositor.CreateScalarKeyFrameAnimation();
                rotationAnimation.InsertKeyFrame(1.0f, 0.0f, _linearEasing);
                rotationAnimation.Duration = TimeSpan.FromMilliseconds(300);

                return rotationAnimation;
            }
            ScalarKeyFrameAnimation CreateShowPanelAnimation()
            {
                var opacityAnimation = Compositor.CreateScalarKeyFrameAnimation();
                opacityAnimation.InsertKeyFrame(0.499f, 0.0f, _linearEasing);
                opacityAnimation.InsertKeyFrame(0.5f, 1.0f, _linearEasing);
                opacityAnimation.Duration = TimeSpan.FromMilliseconds(300);

                return opacityAnimation;
            }
            ScalarKeyFrameAnimation CreateHidePanelAnimation()
            {
                var opacityAnimation = Compositor.CreateScalarKeyFrameAnimation();
                opacityAnimation.InsertKeyFrame(0.5f, 1.0f, _linearEasing);
                opacityAnimation.InsertKeyFrame(0.501f, 0.0f, _linearEasing);
                opacityAnimation.Duration = TimeSpan.FromMilliseconds(300);

                return opacityAnimation;
            }

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
            Checked += OnChecked;
            Unchecked += OnUnchecked;
            PointerEntered += OnPointerEntered;
            AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
            AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
            PointerExited += OnPointerExited;
            PointerCanceled += OnPointerExited;

            if (IsChecked.HasValue && IsChecked.Value)
            {
                GoToCheckedState();
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Equals(e.NewSize)) return;

            var pointerOverPanelHeight = (float)_pointerOverContent.ActualHeight;
            _pointerOverContentHeight.InsertScalar("Value", pointerOverPanelHeight);

            var bottomClip = Compositor.CreateInsetClip(0, 0, 0, pointerOverPanelHeight);
            VisualExtensions.GetVisual(_pointerOverContent).Clip = bottomClip;

            _backPanelVisual.CenterPoint = new Vector3(_backPanel.RenderSize.ToVector2() / 2, 0.0f);
            _visual.CenterPoint = new Vector3(RenderSize.ToVector2() / 2, 0.0f);
        }

        private void OnChecked(object sender, RoutedEventArgs e) => 
            GoToCheckedState();

        private void OnUnchecked(object sender, RoutedEventArgs e) => 
            GoToUncheckedState();

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
            _backgroundShadow.StartShadowBlurRadiusAnimation(_shadowColor, toShadowOpacity: 0.3f, toBlurRadius: 36.0f, duration: 600);
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _visual.StartScaleAnimation(to: new Vector2(0.98f), duration: 300);
            _backgroundShadow.StartShadowBlurRadiusAnimation(_shadowColor, toShadowOpacity: 0.4f, toBlurRadius: 24.0f, duration: 300);
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _visual.StartScaleAnimation(to: new Vector2(1.02f), duration: 200);
            _backgroundShadow.StartShadowBlurRadiusAnimation(_shadowColor, toShadowOpacity: 0.3f, toBlurRadius: 36.0f, duration: 300);
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            _pointerOverContentVisual.Clip.StartAnimation(nameof(InsetClip.BottomInset), _hidePointerOverPanelAnimation);

            _visual.StartScaleAnimation(to: Vector2.One, duration: 300);
            _backgroundShadow.StartShadowBlurRadiusAnimation(_shadowColor, toShadowOpacity: 0, toBlurRadius: 0, duration: 700, delay: 100);
        }

        private void GoToCheckedState()
        {
            _visual.StartAnimation(nameof(Visual.RotationAngleInDegrees), _rotateToBackAnimation);

            _frontPanelVisual.StartAnimation(nameof(Visual.Opacity), _hidePanelAnimation);
            _backPanelVisual.StartAnimation(nameof(Visual.Opacity), _showPanelAnimation);
        }

        private void GoToUncheckedState()
        {
            _visual.StartAnimation(nameof(Visual.RotationAngleInDegrees), _rotateToFrontAnimation);

            _frontPanelVisual.StartAnimation(nameof(Visual.Opacity), _showPanelAnimation);
            _backPanelVisual.StartAnimation(nameof(Visual.Opacity), _hidePanelAnimation);
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