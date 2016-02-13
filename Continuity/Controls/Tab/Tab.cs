using Continuity.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace Continuity.Controls
{
    public sealed class Tab : ItemsControl
    {
        #region Fields

        private const string TOUCH_MOVEMENT_DIRECTION = "ScrollingProperties.Translation.X + SelectedIndex * FullWidth < 0";
        private const string TOUCH_MOVEMENT_PCT_EXPRESSION = "(ScrollingProperties.Translation.X + SelectedIndex * FullWidth) / FullWidth";

        private ScrollViewer _scrollViewer;
        private Border _headersPanelHost;
        private StackPanel _headersPanel;
        private Rectangle _underline;
        private bool _isAnimating;

        private Compositor _compositor;
        private CompositionPropertySet _scrollingProperties;
        private Visual _underlineVisual;
        private ExpressionAnimation _underlineOffsetAnimation, _underlineScaleAnimation;
        private ExpressionAnimation _currentHeaderOpacityAnimation, _nextHeaderOpacityAnimation, _previousHeaderOpacityAnimation;

        #endregion

        public Tab()
        {
            DefaultStyleKey = typeof(Tab);
        }

        #region Events

        public event TabSelectionChangedEventHandler SelectionChanged;

        #endregion

        #region Properties

        private UIElementCollection Headers
        {
            get { return _headersPanel.Children; }
        }

        private List<TabHeaderItem> TypedHeaders
        {
            get { return Headers.OfType<TabHeaderItem>().ToList(); }
        }

        private ScrollViewer ScrollViewer
        {
            get { return _scrollViewer; }
            set
            {
                if (_scrollViewer != null)
                {
                    _scrollViewer.DirectManipulationStarted -= OnScrollViewerDirectManipulationStarted;
                    _scrollViewer.DirectManipulationCompleted -= OnScrollViewerDirectManipulationCompleted;
                }

                _scrollViewer = value;

                if (_scrollViewer != null)
                {
                    _scrollViewer.DirectManipulationStarted += OnScrollViewerDirectManipulationStarted;
                    _scrollViewer.DirectManipulationCompleted += OnScrollViewerDirectManipulationCompleted;
                }
            }
        }

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(Tab),
                new PropertyMetadata(null, (s, dp) =>
                {
                    var self = (Tab)s;
                    if (self._headersPanel == null) return;

                    foreach (TabHeaderItem header in self.Headers)
                    {
                        header.ContentTemplate = dp.NewValue as DataTemplate;
                    }
                }));

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Tab),
                new PropertyMetadata(0, async (s, dp) =>
                {
                    var self = (Tab)s;
                    var oldIndex = (int)dp.OldValue;
                    var newIndex = (int)dp.NewValue;

                    self.SelectedItem = self.Items[newIndex];

                    self.SelectionChanged?.Invoke(self, new TabSelectionChangedEventArgs(oldIndex, newIndex));
                    self.SyncCheckedTabHeaderItem(newIndex);
                    UpdateScrollViewerHorizontalOffset(self, newIndex);
                    await self.SyncUnderlineVisual();
                }));

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(Tab),
                new PropertyMetadata(null, (s, dp) =>
                {
                    var self = (Tab)s;
                    var newIndex = self.Items.IndexOf(dp.NewValue);

                    self.SelectedIndex = newIndex;
                }));

        #endregion

        #region Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ScrollViewer = GetTemplateChild<ScrollViewer>("ScrollViewer");
            _headersPanel = GetTemplateChild<StackPanel>("HeadersPanel");
            _headersPanelHost = GetTemplateChild<Border>("HeadersPanelHost");
            _underline = GetTemplateChild<Rectangle>("HeaderUnderline");

            //ScrollViewer.RegisterPropertyChangedCallback(ScrollViewer.HorizontalOffsetProperty, (s, dp) =>
            //{
            //    Debug.WriteLine($"HorizontalOffset {ScrollViewer.HorizontalOffset}");
            //});

            InitializeCombositionObjects();

            SizeChanged += (s, e) =>
            {
                foreach (var item in Items)
                {
                    var tabItem = ContainerFromItem(item) as TabItem;

                    if (tabItem != null)
                    {
                        tabItem.Width = ActualWidth;
                        tabItem.Height = ActualHeight;

                        UpdateScrollViewerHorizontalOffset(this, SelectedIndex);
                    }
                }
            };

            Loaded += (sender, args) =>
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    var tabItem = ContainerFromIndex(i) as TabItem;

                    if (tabItem != null)
                    {
                        var header = new TabHeaderItem
                        {
                            Content = tabItem.Header,
                            ContentTemplate = HeaderTemplate,
                            IsChecked = i == 0
                        };

                        header.Checked += (s, e) =>
                        {
                            UpdateHeaderVisuals();

                            var h = (TabHeaderItem)s;
                            SelectedIndex = GetHeaderIndex(h);
                        };

                        header.Unchecked += (s, e) =>
                        {
                            UpdateHeaderVisuals();
                        };

                        header.SizeChanged += async (s, e) =>
                        {
                            UpdateHeaderVisuals();

                            await SyncUnderlineVisual();
                        };

                        Headers.Add(header);
                    }
                }
            };
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TabItem();
        }

        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
        }

        #endregion

        #region Handlers

        private async void OnScrollViewerDirectManipulationStarted(object sender, object e)
        {
            _isAnimating = true;
            ScrollViewer.HorizontalSnapPointsType = SnapPointsType.MandatorySingle;

            var currentHeader = TypedHeaders[SelectedIndex];
            var previousHeader = SelectedIndex > 0 ? TypedHeaders[SelectedIndex - 1] : null;
            var nextHeader = SelectedIndex < Headers.Count - 1 ? TypedHeaders[SelectedIndex + 1] : null;

            // Create offset animation
            var startingOffsetX = _underlineVisual.Offset.X;
            var toNextOffsetX = nextHeader == null ? 0 : nextHeader.OffsetX(currentHeader);
            var toPreviousOffsetX = previousHeader == null ? 0 : previousHeader.OffsetX(currentHeader);

            _underlineOffsetAnimation = _compositor.CreateExpressionAnimation($"{TOUCH_MOVEMENT_DIRECTION} ? StartingOffsetX - ToNextOffsetX * {TOUCH_MOVEMENT_PCT_EXPRESSION} : StartingOffsetX + ToPreviousOffsetX * {TOUCH_MOVEMENT_PCT_EXPRESSION}");
            _underlineOffsetAnimation.SetScalarParameter("StartingOffsetX", startingOffsetX);
            _underlineOffsetAnimation.SetScalarParameter("ToNextOffsetX", toNextOffsetX);
            _underlineOffsetAnimation.SetScalarParameter("ToPreviousOffsetX", toPreviousOffsetX);
            SetSharedParameters(_underlineOffsetAnimation);

            // Create scale animation
            var startingScaleX = _underlineVisual.Scale.X;
            var nextAndCurrentWidthDiff = nextHeader == null ? 0 : (GetHeaderTextBlock(nextHeader).ActualWidth - GetHeaderTextBlock(currentHeader).ActualWidth).ToFloat();
            var previousAndCurrentWidthDiff = previousHeader == null ? 0 : (GetHeaderTextBlock(previousHeader).ActualWidth - GetHeaderTextBlock(currentHeader).ActualWidth).ToFloat();

            _underlineScaleAnimation = _compositor.CreateExpressionAnimation($"{TOUCH_MOVEMENT_DIRECTION} ? StartingScaleX - NextAndCurrentWidthDiff * {TOUCH_MOVEMENT_PCT_EXPRESSION} : StartingScaleX + PreviousAndCurrentWidthDiff * {TOUCH_MOVEMENT_PCT_EXPRESSION}");
            _underlineScaleAnimation.SetScalarParameter("StartingScaleX", startingScaleX);
            _underlineScaleAnimation.SetScalarParameter("NextAndCurrentWidthDiff", nextAndCurrentWidthDiff);
            _underlineScaleAnimation.SetScalarParameter("PreviousAndCurrentWidthDiff", previousAndCurrentWidthDiff);
            SetSharedParameters(_underlineScaleAnimation);

            // Create opacity animations
            _currentHeaderOpacityAnimation = _compositor.CreateExpressionAnimation($"max(1 - {TOUCH_MOVEMENT_PCT_EXPRESSION} * ({TOUCH_MOVEMENT_DIRECTION} ? -1.0f : 1.0f), UncheckedStateOpacity)");
            SetSharedParameters(_currentHeaderOpacityAnimation);

            _nextHeaderOpacityAnimation = _compositor.CreateExpressionAnimation($"{TOUCH_MOVEMENT_DIRECTION} ? UncheckedStateOpacity - {TOUCH_MOVEMENT_PCT_EXPRESSION} : UncheckedStateOpacity");
            SetSharedParameters(_nextHeaderOpacityAnimation);

            _previousHeaderOpacityAnimation = _compositor.CreateExpressionAnimation($"{TOUCH_MOVEMENT_DIRECTION} ? UncheckedStateOpacity : UncheckedStateOpacity + {TOUCH_MOVEMENT_PCT_EXPRESSION}");
            SetSharedParameters(_previousHeaderOpacityAnimation);

            // Start all animations
            _underlineVisual.StartAnimation("Offset.X", _underlineOffsetAnimation);
            _underlineVisual.StartAnimation("Scale.X", _underlineScaleAnimation);

            currentHeader.Visual().StartAnimation("Opacity", _currentHeaderOpacityAnimation);
            if (previousHeader != null) previousHeader.Visual().StartAnimation("Opacity", _previousHeaderOpacityAnimation);
            if (nextHeader != null) nextHeader.Visual().StartAnimation("Opacity", _nextHeaderOpacityAnimation);

            // Don't allow to swipe too fast.
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

        #endregion

        #region Methods

        private void InitializeCombositionObjects()
        {
            _scrollingProperties = ScrollViewer.ScrollProperties();
            _compositor = _scrollingProperties.Compositor;
            _underlineVisual = _underline.Visual();
        }

        private int CalculateSelectedIndexBasedOnScrollViewerHorizontalOffset()
        {
            var index = ScrollViewer.HorizontalOffset / ActualWidth;
            return (int)(Math.Round(index, MidpointRounding.AwayFromZero));
        }

        private static void UpdateScrollViewerHorizontalOffset(Tab self, int newIndex)
        {
            self.ScrollViewer.HorizontalSnapPointsType = SnapPointsType.Mandatory;
            self.ScrollViewer.ChangeView(self.ActualWidth * newIndex, null, null, false);
        }

        private void SyncCheckedTabHeaderItem(int index)
        {
            var header = TypedHeaders.Single((h) => GetHeaderIndex(h) == index);
            header.IsChecked = true;
        }

        private async Task SyncUnderlineVisual()
        {
            var header = TypedHeaders.Single((h) => h.IsChecked == true);

            if (GetHeaderIndex(header) == SelectedIndex)
            {
                var textBlock = GetHeaderTextBlock(header);
                var offsetX = textBlock.OffsetX(_headersPanelHost);
                var scaleX = textBlock.ActualWidth.ToFloat(); // The ActualWidth of the HeaderUnderline Rectangle is 1 so I ignored the /1 here

                var offsetTask = _underline.StartOffsetAnimation(AnimationAxis.X, offsetX, 400, easing: _compositor.CreateEaseInOutCubic());
                var scaleTask = _underline.StartScaleAnimation(AnimationAxis.X, scaleX, 400, easing: _compositor.CreateEaseInOutCubic());
                await Task.WhenAll(offsetTask, scaleTask);
            }
        }

        private void SetSharedParameters(ExpressionAnimation animation)
        {
            animation.SetReferenceParameter("ScrollingProperties", _scrollingProperties);
            animation.SetScalarParameter("FullWidth", ActualWidth.ToFloat());
            animation.SetScalarParameter("SelectedIndex", SelectedIndex);
            animation.SetScalarParameter("UncheckedStateOpacity", 0.4f);
        }

        private void UpdateHeaderVisuals()
        {
            foreach (var header in TypedHeaders)
            {
                header.Visual().Opacity = header.IsChecked.Value ? 1.0f : 0.4f;
            }
        }

        private TextBlock GetHeaderTextBlock(TabHeaderItem header)
        {
            return header.GetChildByName<TextBlock>("Header");
        }

        private int GetHeaderIndex(TabHeaderItem header)
        {
            return Headers.IndexOf(header);
        }

        T GetTemplateChild<T>(string name, string message = null) where T : DependencyObject
        {
            var child = GetTemplateChild(name) as T;

            if (child == null)
            {
                if (message == null)
                {
                    message = $"{name} should not be null! Check the default Generic.xaml.";
                }

                throw new NullReferenceException(message);
            }

            return child;
        }

        #endregion
    }
}
