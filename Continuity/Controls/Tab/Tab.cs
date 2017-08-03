using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using Continuity.Extensions;
using Windows.UI.Xaml.Media;

namespace Continuity.Controls
{
    [TemplatePart(Name = PART_ScrollViewer, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = PART_HeadersPanel, Type = typeof(StackPanel))]
    [TemplatePart(Name = PART_HeadersPanelHost, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = PART_SelectedHeaderIndicatorHost, Type = typeof(Border))]
    [TemplatePart(Name = PART_SelectedHeaderIndicator, Type = typeof(Rectangle))]
    public sealed class Tab : ItemsControl
    {
        #region Fields

        // ReSharper disable once InconsistentNaming
        private const string BUG_FIXING_EXPRESSION_10586_ONLY = "+ (0 + (0 + (0 + (0 + (0 + 0)))))";

        private const string PART_ScrollViewer = "ScrollViewer";
        private const string PART_HeadersPanel = "HeadersPanel";
        private const string PART_HeadersPanelHost = "HeadersPanelHost";
        private const string PART_SelectedHeaderIndicatorHost = "SelectedHeaderIndicatorHost";
        private const string PART_SelectedHeaderIndicator = "SelectedHeaderIndicator";

        private const string Scroller = "ScrollingProperties";
        private static readonly string ScrollerTranslationX = $"{Scroller}.Translation.X";
        private static readonly string DragDistance = $"({ScrollerTranslationX} + SelectedIndex * FullWidth)";
        private static readonly string DragDirection = $"{DragDistance} < 0";
        private static readonly string DragDistancePct = $"{DragDistance} / FullWidth";

        private ScrollViewer _scrollViewer;
        private ScrollViewer _headersPanelHost;
        private StackPanel _headersPanel;
        private Border _selectedHeaderIndicatorHost;
        private Rectangle _selectedHeaderIndicator;
        private bool _isAnimating;
        private bool _isScrolling;

        private Compositor _compositor;
        private CompositionPropertySet _headersScrollingProperties;
        private CompositionPropertySet _contentScrollingProperties;
        private Visual _selectedHeaderIndicatorHostVisual;
        private Visual _selectedHeaderIndicatorVisual;
        private ExpressionAnimation _selectedVisualOffsetAnimation, _selectedVisualScaleAnimation;
        private ExpressionAnimation _currentHeaderOpacityAnimation, _nextHeaderOpacityAnimation, _previousHeaderOpacityAnimation;

        private bool _isLoaded;

        #endregion

        public Tab()
        {
            DefaultStyleKey = typeof(Tab);
        }

        #region Events

        public event TabSelectionChangedEventHandler SelectionChanged;

        #endregion

        #region Properties

        private UIElementCollection Headers => _headersPanel.Children;

        private List<TabHeaderItem> TypedHeaders => Headers.OfType<TabHeaderItem>().ToList();

        private ScrollViewer ScrollViewer
        {
            get => _scrollViewer;
            set
            {
                if (_scrollViewer != null)
                {
                    _scrollViewer.DirectManipulationStarted -= OnScrollViewerDirectManipulationStarted;
                    _scrollViewer.DirectManipulationCompleted -= OnScrollViewerDirectManipulationCompleted;
                }

                _scrollViewer = value;

                // ReSharper disable once InvertIf
                if (_scrollViewer != null)
                {
                    _scrollViewer.DirectManipulationStarted += OnScrollViewerDirectManipulationStarted;
                    _scrollViewer.DirectManipulationCompleted += OnScrollViewerDirectManipulationCompleted;
                }
            }
        }

        private ScrollViewer HeadersPanelHost
        {
            get => _headersPanelHost;
            set
            {
                if (_headersPanelHost != null)
                {
                    _headersPanelHost.DirectManipulationStarted -= OnHeadersPanelHostDirectManipulationStarted;
                    _headersPanelHost.DirectManipulationCompleted -= OnHeadersPanelHostDirectManipulationCompleted;
                }

                _headersPanelHost = value;

                // ReSharper disable once InvertIf
                if (_headersPanelHost != null)
                {
                    _headersPanelHost.DirectManipulationStarted += OnHeadersPanelHostDirectManipulationStarted;
                    _headersPanelHost.DirectManipulationCompleted += OnHeadersPanelHostDirectManipulationCompleted;
                }
            }
        }

        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(Tab),
                new PropertyMetadata(null, (s, dp) =>
                {
                    var self = (Tab)s;
                    if (self._headersPanel == null) return;

                    foreach (var elmement in self.Headers)
                    {
                        var header = (TabHeaderItem)elmement;
                        header.ContentTemplate = dp.NewValue as DataTemplate;
                    }
                }));

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Tab),
                new PropertyMetadata(0, (s, dp) =>
                {
                    var self = (Tab)s;
                    var oldIndex = (int)dp.OldValue;
                    var newIndex = (int)dp.NewValue;

                    self.SelectedItem = self.Items?[newIndex];

                    self.SelectionChanged?.Invoke(self, new TabSelectionChangedEventArgs(oldIndex, newIndex));
                    self.SyncCheckedTabHeaderItem(newIndex);
                    UpdateScrollViewerHorizontalOffset(self, newIndex);
                }));

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(Tab),
                new PropertyMetadata(null, (s, dp) =>
                {
                    var self = (Tab)s;
                    var newIndex = self.Items?.IndexOf(dp.NewValue);

                    self.SelectedIndex = newIndex ?? self.SelectedIndex;
                }));

        public Thickness HeadersPanelMargin
        {
            get => (Thickness)GetValue(HeadersPanelMarginProperty);
            set => SetValue(HeadersPanelMarginProperty, value);
        }
        public static readonly DependencyProperty HeadersPanelMarginProperty =
            DependencyProperty.Register("HeadersPanelMargin", typeof(Thickness), typeof(Tab),
                new PropertyMetadata(new Thickness()));

        public HorizontalAlignment HeadersPanelHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(HeadersPanelHorizontalAlignmentProperty);
            set => SetValue(HeadersPanelHorizontalAlignmentProperty, value);
        }
        public static readonly DependencyProperty HeadersPanelHorizontalAlignmentProperty =
            DependencyProperty.Register("HeadersPanelHorizontalAlignment", typeof(HorizontalAlignment), typeof(Tab),
                new PropertyMetadata(HorizontalAlignment.Left));

        public Brush SelectedHeaderIndicatorBackground
        {
            get => (Brush)GetValue(SelectedHeaderIndicatorBackgroundProperty);
            set => SetValue(SelectedHeaderIndicatorBackgroundProperty, value);
        }
        public static readonly DependencyProperty SelectedHeaderIndicatorBackgroundProperty =
            DependencyProperty.Register("SelectedHeaderIndicatorBackground", typeof(Brush), typeof(Tab),
                new PropertyMetadata(default(Brush)));

        public Brush HeadersPanelBackground
        {
            get => (Brush)GetValue(HeadersPanelBackgroundProperty);
            set => SetValue(HeadersPanelBackgroundProperty, value);
        }
        public static readonly DependencyProperty HeadersPanelBackgroundProperty =
            DependencyProperty.Register("HeadersPanelBackground", typeof(Brush), typeof(Tab),
                new PropertyMetadata(default(Brush)));

        public bool UseLineSelectionVisual
        {
            get => (bool)GetValue(UseLineSelectionVisualProperty);
            set => SetValue(UseLineSelectionVisualProperty, value);
        }
        public static readonly DependencyProperty UseLineSelectionVisualProperty =
            DependencyProperty.Register("UseLineSelectionVisual", typeof(bool), typeof(Tab),
                new PropertyMetadata(false));

        #endregion

        #region Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ScrollViewer = GetTemplateChild<ScrollViewer>(PART_ScrollViewer);
            _headersPanel = GetTemplateChild<StackPanel>(PART_HeadersPanel);
            HeadersPanelHost = GetTemplateChild<ScrollViewer>(PART_HeadersPanelHost);
            _selectedHeaderIndicatorHost = GetTemplateChild<Border>(PART_SelectedHeaderIndicatorHost);
            _selectedHeaderIndicator = GetTemplateChild<Rectangle>(PART_SelectedHeaderIndicator);

            _selectedHeaderIndicatorHost.Margin = UseLineSelectionVisual ? new Thickness() : new Thickness(0, 8, 0, 0);
            _selectedHeaderIndicatorHost.VerticalAlignment = UseLineSelectionVisual ? VerticalAlignment.Bottom : VerticalAlignment.Stretch;
            _selectedHeaderIndicator.Height = UseLineSelectionVisual ? 3.0d : double.NaN;

            InitializeCompositionObjects();

            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
        }

        protected override DependencyObject GetContainerForItemOverride() => 
            new TabItem();

        #endregion

        #region Handlers

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Items == null) return;

            _selectedHeaderIndicatorHostVisual.Size = new Vector2(_selectedHeaderIndicatorHost.ActualWidth.ToFloat(),
                _selectedHeaderIndicatorHost.ActualHeight.ToFloat());

            foreach (var item in Items)
            {
                var tabItem = ContainerFromItem(item) as TabItem;
                if (tabItem == null) continue;

                tabItem.Width = ActualWidth;
                UpdateScrollViewerHorizontalOffset(this, SelectedIndex);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            if (Items == null) return;

            SetSelectedHeaderIndicatorVisualBoundaries();

            for (var i = 0; i < Items.Count; i++)
            {
                var tabItem = ContainerFromIndex(i) as TabItem;

                if (tabItem == null) continue;

                var header = new TabHeaderItem
                {
                    DataContext = Items[i],
                    Content = tabItem.Header,
                    HeaderIconStyle = tabItem.HeaderIconStyle,
                    ContentTemplate = HeaderTemplate,
                    IsChecked = i == 0
                };

                // Have to do this to avoid a bug where RadioButton's GroupName 
                // doesn't function properly after Reloaded.
                header.Loaded += async (s, args) =>
                {
                    // TODO: There should be a better way to handle this, at system level??
                    // Put a short delay here to allow the system to calculate the Offset
                    // before we can animate it.
                    await Task.Delay(1000);

                    _isLoaded = true;

                    var h = (TabHeaderItem)s;
                    h.GroupName = "Headers";

                    UpdateHeaderVisuals();
                    SyncSelectedHeaderIndicatorVisual();
                };
                header.Unloaded += (s, args) =>
                {
                    var h = (TabHeaderItem)s;
                    h.GroupName = string.Empty;
                };

                header.Checked += async (s, args) =>
                {
                    UpdateHeaderVisuals();

                    var h = (TabHeaderItem)s;
                    SelectedIndex = GetHeaderIndex(h);

                    // TODO: We might be able to monitor the scrolling on the HeadersPanelHost and sync the selection visual with the scrolling in real time.
                    // OnHeadersPanelHostDirectManipulationStarted(null, null);
                    await HeadersPanelHost.ScrollToElementAsync(header, false, bringToTopOrLeft: false);
                    SyncSelectedHeaderIndicatorVisual();
                };
                header.Unchecked += (s, args) =>
                {
                    UpdateHeaderVisuals();
                };

                header.SizeChanged += (s, args) =>
                {
                    if (!_isLoaded) return;

                    UpdateHeaderVisuals();
                    SyncSelectedHeaderIndicatorVisual();
                };

                Headers.Add(header);
            }
        }

        private async void OnScrollViewerDirectManipulationStarted(object sender, object e)
        {
            if (_isScrolling) return;

            _isAnimating = true;
            ScrollViewer.HorizontalSnapPointsType = SnapPointsType.MandatorySingle;

            var currentHeader = TypedHeaders[SelectedIndex];
            var previousHeader = SelectedIndex > 0 ? TypedHeaders[SelectedIndex - 1] : null;
            var nextHeader = SelectedIndex < Headers.Count - 1 ? TypedHeaders[SelectedIndex + 1] : null;

            // Create offset animation
            var startingOffsetX = _selectedHeaderIndicatorVisual.Offset.X;
            var toNextOffsetX = nextHeader?.RelativePosition(currentHeader).X.ToFloat() ?? 0;
            var toPreviousOffsetX = previousHeader?.RelativePosition(currentHeader).X.ToFloat() ?? 0;

            _selectedVisualOffsetAnimation = _compositor.CreateExpressionAnimation($"{DragDirection} {BUG_FIXING_EXPRESSION_10586_ONLY} ? StartingOffsetX - ToNextOffsetX * {DragDistancePct} : StartingOffsetX + ToPreviousOffsetX * {DragDistancePct}");
            _selectedVisualOffsetAnimation.SetScalarParameter("StartingOffsetX", startingOffsetX);
            _selectedVisualOffsetAnimation.SetScalarParameter("ToNextOffsetX", toNextOffsetX);
            _selectedVisualOffsetAnimation.SetScalarParameter("ToPreviousOffsetX", toPreviousOffsetX);
            SetSharedParameters(_selectedVisualOffsetAnimation);

            // Create scale animation
            var startingScaleX = _selectedHeaderIndicatorVisual.Scale.X;
            var nextAndCurrentWidthDiff = nextHeader == null ? 0 : (GetHeaderContainer(nextHeader).ActualWidth - GetHeaderContainer(currentHeader).ActualWidth).ToFloat();
            var previousAndCurrentWidthDiff = previousHeader == null ? 0 : (GetHeaderContainer(previousHeader).ActualWidth - GetHeaderContainer(currentHeader).ActualWidth).ToFloat();

            _selectedVisualScaleAnimation = _compositor.CreateExpressionAnimation($"{DragDirection} {BUG_FIXING_EXPRESSION_10586_ONLY} ? StartingScaleX - NextAndCurrentWidthDiff * {DragDistancePct} : StartingScaleX + PreviousAndCurrentWidthDiff * {DragDistancePct}");
            _selectedVisualScaleAnimation.SetScalarParameter("StartingScaleX", startingScaleX);
            _selectedVisualScaleAnimation.SetScalarParameter("NextAndCurrentWidthDiff", nextAndCurrentWidthDiff);
            _selectedVisualScaleAnimation.SetScalarParameter("PreviousAndCurrentWidthDiff", previousAndCurrentWidthDiff);
            SetSharedParameters(_selectedVisualScaleAnimation);

            // Create opacity animations
            _currentHeaderOpacityAnimation = _compositor.CreateExpressionAnimation($"Max(1 - Abs{DragDistancePct}, UncheckedStateOpacity)");
            SetSharedParameters(_currentHeaderOpacityAnimation);

            _nextHeaderOpacityAnimation = _compositor.CreateExpressionAnimation($"{DragDirection} {BUG_FIXING_EXPRESSION_10586_ONLY} ? UncheckedStateOpacity - {DragDistancePct} : UncheckedStateOpacity");
            SetSharedParameters(_nextHeaderOpacityAnimation);

            _previousHeaderOpacityAnimation = _compositor.CreateExpressionAnimation($"{DragDirection} {BUG_FIXING_EXPRESSION_10586_ONLY} ? UncheckedStateOpacity : UncheckedStateOpacity + {DragDistancePct}");
            SetSharedParameters(_previousHeaderOpacityAnimation);

            // Start all animations
            _selectedHeaderIndicatorVisual.StartAnimation("Offset.X", _selectedVisualOffsetAnimation);
            _selectedHeaderIndicatorVisual.StartAnimation("Scale.X", _selectedVisualScaleAnimation);

            currentHeader.Visual().StartAnimation("Opacity", _currentHeaderOpacityAnimation);
            previousHeader?.Visual().StartAnimation("Opacity", _previousHeaderOpacityAnimation);
            nextHeader?.Visual().StartAnimation("Opacity", _nextHeaderOpacityAnimation);

            // Don't allow swiping too fast. This is all because we don't have this -
            // https://github.com/Microsoft/WindowsUIDevLabs/issues/181
            IsHitTestVisible = false;
            while (_isAnimating)
            {
                await Task.Delay(50);
            }
            IsHitTestVisible = true;
        }

        private void OnScrollViewerDirectManipulationCompleted(object sender, object e)
        {
            SelectedIndex = CalculateSelectedIndexBasedOnScrollViewerHorizontalOffset();
            _isAnimating = false;

            Debug.WriteLine($"Completed {SelectedIndex}");
        }

        private void OnHeadersPanelHostDirectManipulationStarted(object sender, object e)
        {
            if (_isAnimating) return;

            _isScrolling = true;

            var header = TypedHeaders.Single(h => h.IsChecked == true);
            if (GetHeaderIndex(header) != SelectedIndex) return;

            var headerContainer = GetHeaderContainer(header);
            // _headersPanel goes over the its parent ScrollViewer's viewport, so we need to use it.
            var offsetX = headerContainer.RelativePosition(_headersPanel).X.ToFloat();

            var offsetAnimation = _compositor.CreateExpressionAnimation($"CurrentOffsetX + {ScrollerTranslationX} + LeftMargin");
            offsetAnimation.SetReferenceParameter(Scroller, _headersScrollingProperties);
            offsetAnimation.SetScalarParameter("LeftMargin", 12.0f);
            offsetAnimation.SetScalarParameter("CurrentOffsetX", offsetX);
            _selectedHeaderIndicatorVisual.StartAnimation("Offset.X", offsetAnimation);
        }

        private void OnHeadersPanelHostDirectManipulationCompleted(object sender, object e)
        {
            _isScrolling = false;

            _selectedHeaderIndicatorVisual.StopAnimation("Offset.X");
            SyncSelectedHeaderIndicatorVisual();
        }

        #endregion

        #region Methods

        private void InitializeCompositionObjects()
        {
            _headersScrollingProperties = HeadersPanelHost.ScrollProperties();
            _contentScrollingProperties = ScrollViewer.ScrollProperties();
            _compositor = _headersScrollingProperties.Compositor;
            _selectedHeaderIndicatorVisual = _selectedHeaderIndicator.Visual();
            _selectedHeaderIndicatorHostVisual = _selectedHeaderIndicatorHost.Visual();
        }

        private void SetSelectedHeaderIndicatorVisualBoundaries()
        {
            var clip = _compositor.CreateInsetClip();
            clip.LeftInset = HeadersPanelMargin.Left.ToFloat();
            clip.RightInset = HeadersPanelMargin.Right.ToFloat();
            clip.TopInset = HeadersPanelMargin.Top.ToFloat();
            clip.BottomInset = HeadersPanelMargin.Bottom.ToFloat();

            _selectedHeaderIndicatorHostVisual.Clip = clip;
        }

        private int CalculateSelectedIndexBasedOnScrollViewerHorizontalOffset()
        {
            var index = ScrollViewer.HorizontalOffset / ActualWidth;
            return (int)Math.Round(index, MidpointRounding.AwayFromZero);
        }

        private static void UpdateScrollViewerHorizontalOffset(Tab self, int newIndex)
        {
            self.ScrollViewer.HorizontalSnapPointsType = SnapPointsType.Mandatory;
            self.ScrollViewer.ChangeView(self.ActualWidth * newIndex, null, null, false);
        }

        private void SyncCheckedTabHeaderItem(int index)
        {
            var header = TypedHeaders.Single(h => GetHeaderIndex(h) == index);
            header.IsChecked = true;
        }

        private void SyncSelectedHeaderIndicatorVisual()
        {
            var header = TypedHeaders.Single(h => h.IsChecked == true);
            if (GetHeaderIndex(header) != SelectedIndex) return;

            var container = GetHeaderContainer(header);
            var offsetX = container.RelativePosition(HeadersPanelHost).X.ToFloat() + (UseLineSelectionVisual ? 11.0f : 0);
            // The ActualWidth of the selected header indication Rectangle is 1 so I ignored the /1 here.
            var scaleX = container.ActualWidth.ToFloat() - (UseLineSelectionVisual ? 24.0f : 0);

            _selectedHeaderIndicatorVisual.StartOffsetAnimation(AnimationAxis.X, null, offsetX, 400, easing: _compositor.EaseInOutCubic());
            _selectedHeaderIndicatorVisual.StartScaleAnimation(AnimationAxis.X, null, scaleX, 400, easing: _compositor.EaseInOutCubic());
        }

        private void SetSharedParameters(ExpressionAnimation animation)
        {
            animation.SetReferenceParameter(Scroller, _contentScrollingProperties);
            animation.SetScalarParameter("FullWidth", ActualWidth.ToFloat());
            animation.SetScalarParameter("SelectedIndex", SelectedIndex);
            animation.SetScalarParameter("UncheckedStateOpacity", 0.4f);
        }

        private void UpdateHeaderVisuals()
        {
            foreach (var header in TypedHeaders)
            {
                if (header.IsChecked.HasValue && header.IsChecked.Value)
                {
                    header.Visual().StartOpacityAnimation(null, 1.0f, 150.0f);
                }
                else
                {
                    header.Visual().StartOpacityAnimation(null, 0.4f, 150.0f);
                }
            }
        }

        private static FrameworkElement GetHeaderContainer(TabHeaderItem header)
        {
            var container = header.Children().FirstOrDefault();

            if (container == null)
            {
                throw new NullReferenceException("No header content found.");
            }

            return container;
        }

        private int GetHeaderIndex(TabHeaderItem header)
        {
            return Headers.IndexOf(header);
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
