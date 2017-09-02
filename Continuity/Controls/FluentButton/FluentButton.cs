using Continuity.Lights;
using System;
using System.Linq;
using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;
using Continuity.Extensions;

namespace Continuity.Controls
{
    [TemplatePart(Name = PART_RootGrid, Type = typeof(Grid))]
    [TemplatePart(Name = PART_BackgroundShadowContainer, Type = typeof(Rectangle))]
    [TemplatePart(Name = PART_TextShadowContainer, Type = typeof(Rectangle))]
    [TemplatePart(Name = PART_ContentPresenter, Type = typeof(ContentPresenter))]
    public sealed class FluentButton : Button
    {
        #region Fields

        private const string PART_RootGrid = "RootGrid";
        private const string PART_BackgroundShadowContainer = "BackgroundShadowContainer";
        private const string PART_TextShadowContainer = "TextShadowContainer";
        private const string PART_ContentPresenter = "ContentPresenter";

        private Grid _rootGrid;
        private Rectangle _backgroundShadowContainer;
        private Rectangle _textShadowContainer;
        private ContentPresenter _contentPresenter;
        private TextBlock _contentTextBlock;

        private readonly Compositor _compositor;
        private readonly DropShadow _backgroundShadow;
        private readonly DropShadow _textShadow;

        #endregion

        public FluentButton()
        {
            DefaultStyleKey = typeof(FluentButton);

            _compositor = Window.Current.Compositor;
            _backgroundShadow = CreateDropShadow();
            _textShadow = CreateDropShadow();

            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
            PointerEntered += OnPointerEntered;
            AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
            AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
            PointerExited += OnPointerExited;
            PointerCanceled += OnPointerExited;
            Click += OnClick;
            Holding += OnHolding;
        }

        #region Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _rootGrid = GetTemplateChild<Grid>(PART_RootGrid);
            _backgroundShadowContainer = GetTemplateChild<Rectangle>(PART_BackgroundShadowContainer);
            _textShadowContainer = GetTemplateChild<Rectangle>(PART_TextShadowContainer);
            _contentPresenter = GetTemplateChild<ContentPresenter>(PART_ContentPresenter);

            _backgroundShadowContainer.SetDropShadow(_backgroundShadow);
            _textShadowContainer.SetDropShadow(_textShadow);

            Lights.Add(new HoverXamlLight());
            Lights.Add(new AmbientXamlLight());
        }

        #endregion

        #region Event Handlers

        private void OnLoaded(object sender, RoutedEventArgs e) => 
            _contentTextBlock = _contentPresenter.Children().OfType<TextBlock>().FirstOrDefault();

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Equals(e.NewSize)) return;
            _rootGrid.Visual().CenterPoint = new Vector3(_rootGrid.RenderSize.ToVector2() / 2, 0.0f);
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _rootGrid.StartScaleAnimation(to: new Vector2(1.025f), duration: 400);
            _backgroundShadow.StartShadowBlurRadiusAnimation(toShadowOpacity: 0.3f, toBlurRadius: 8.0f, duration: 600);
            _textShadow.StartShadowBlurRadiusAnimation(toShadowOpacity: 0.2f, toBlurRadius: 2.0f, duration: 300, delay: 200, maskingElement: _contentTextBlock);
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _rootGrid.StartScaleAnimation(to: new Vector2(0.975f), duration: 300);
            _backgroundShadow.StartShadowBlurRadiusAnimation(toShadowOpacity: 0.4f, toBlurRadius: 6.0f, duration: 300);
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _rootGrid.StartScaleAnimation(to: new Vector2(1.025f), duration: 200);
            _backgroundShadow.StartShadowBlurRadiusAnimation(toShadowOpacity: 0.3f, toBlurRadius: 8.0f, duration: 300);
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            _rootGrid.StartScaleAnimation(to: Vector2.One, duration: 300);
            _backgroundShadow.StartShadowBlurRadiusAnimation(toShadowOpacity: 0.0f, toBlurRadius: 0.0f, duration: 500, delay: 100);
            _textShadow.StartShadowBlurRadiusAnimation(toShadowOpacity: 0.0f, toBlurRadius: 0.0f, duration: 300, maskingElement: _contentTextBlock);
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void OnHolding(object sender, HoldingRoutedEventArgs e)
        {
        }

        #endregion

        #region Methods

        private DropShadow CreateDropShadow(Color? color = null)
        {
            var shadow = _compositor.CreateDropShadow();

            shadow.BlurRadius = 0.0f;
            shadow.Opacity = 0.0f;

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

        #endregion
    }
}