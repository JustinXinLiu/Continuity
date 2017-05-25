using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Animation;
using Continuity.Extensions;

namespace Continuity.Controls
{
    /// <summary>
    /// This is built on top of the AdaptiveGridView from the UWP Community Toolkit. The only enhancement is that
    /// it adds a nice staggered diagonal entrance transition to the list.
    /// 
    /// The AdaptiveGridView control allows to present information within a Grid View perfectly adjusting the
    /// total display available space. It reacts to changes in the layout as well as the content so it can adapt
    /// to different form factors automatically.
    /// </summary>
    /// <remarks>
    /// The number and the width of items are calculated based on the
    /// screen resolution in order to fully leverage the available screen space. The property ItemsHeight define
    /// the items fixed height and the property DesiredWidth sets the minimum width for the elements to add a
    /// new column.</remarks>
    public partial class AdaptiveGridView : GridView
    {
        private bool _isLoaded;
        private ScrollMode _savedVerticalScrollMode;
        private ScrollMode _savedHorizontalScrollMode;
        private ScrollBarVisibility _savedVerticalScrollBarVisibility;
        private ScrollBarVisibility _savedHorizontalScrollBarVisibility;
        private Orientation _savedOrientation;
        private bool _needToRestoreScrollStates;

        private bool _isAnimated;
        private readonly Dictionary<FrameworkElement, Position> _visibleItemContainers = new Dictionary<FrameworkElement, Position>();
        private readonly Vector3KeyFrameAnimation _scaleAnimation;

        // Can be made as dependency properties
        private const int InitialDelay = 400;
        private const int AnimationDuration = 360;
        private const int AnimationDelay = 100;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdaptiveGridView"/> class.
        /// </summary>
        public AdaptiveGridView()
        {
            IsTabStop = false;
            SizeChanged += OnSizeChanged;
            ItemClick += OnItemClick;
            if (Items != null) Items.VectorChanged += ItemsOnVectorChanged;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;

            // Define ItemContainerStyle in code rather than using the DefaultStyle
            // to avoid having to define the entire style of a GridView. This can still
            // be set by the enduser to values of their chosing
            var style = new Style(typeof(GridViewItem));
            style.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));
            style.Setters.Add(new Setter(VerticalContentAlignmentProperty, VerticalAlignment.Stretch));
            ItemContainerStyle = style;

            // Set up opacity and scale animations.
            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _scaleAnimation = compositor.CreateVector3KeyFrameAnimation();
            _scaleAnimation.Duration = TimeSpan.FromMilliseconds(AnimationDuration);
            _scaleAnimation.InsertKeyFrame(0, new Vector3(0.8f, 0.8f, 0));
            _scaleAnimation.InsertKeyFrame(1, Vector3.One, EaseOutCubic(compositor));

            // Remove the default entrance transition if existed.
            RegisterPropertyChangedCallback(ItemContainerTransitionsProperty, (s, e) =>
            {
                var entranceThemeTransition = ItemContainerTransitions.OfType<EntranceThemeTransition>().SingleOrDefault();
                if (entranceThemeTransition != null)
                {
                    ItemContainerTransitions.Remove(entranceThemeTransition);
                }
            });
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="obj">The element that's used to display the specified item.</param>
        /// <param name="item">The item to display.</param>
        protected override async void PrepareContainerForItemOverride(DependencyObject obj, object item)
        {
            base.PrepareContainerForItemOverride(obj, item);
            var element = obj as FrameworkElement;
            if (element == null) return;

            var heightBinding = new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(ItemHeight)),
                Mode = BindingMode.TwoWay
            };

            var widthBinding = new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(ItemWidth)),
                Mode = BindingMode.TwoWay
            };

            element.SetBinding(HeightProperty, heightBinding);
            element.SetBinding(WidthProperty, widthBinding);

            var itemsWrapGrid = ItemsPanelRoot as ItemsWrapGrid;
            if (itemsWrapGrid == null) return;

            var numberOfColumns = (int)Math.Round(ActualWidth / ItemWidth);
            var index = IndexFromContainer(obj);
            var rowIndex = index % numberOfColumns;
            var colIndex = index / numberOfColumns;

            if (itemsWrapGrid.LastVisibleIndex <= 0)
            {
                element.Opacity = 0;

                if (_visibleItemContainers.ContainsKey(element))
                {
                    _visibleItemContainers.Remove(element);
                }
                _visibleItemContainers.Add(element, new Position(rowIndex, colIndex));

                // When all items are visible, run the staggered animation in the end.
                if (!_isAnimated && index == Items?.Count - 1)
                {
                    await RunStaggeredAnimationOnItemContainersAsync(numberOfColumns);
                }
            }
            else
            {
                if (_isAnimated) return;
                _isAnimated = true;

                // Only run the staggered animation once when all visible items are rendered.
                await RunStaggeredAnimationOnItemContainersAsync(numberOfColumns);
            }
        }

        private async Task RunStaggeredAnimationOnItemContainersAsync(int numberOfColumns)
        {
            await Task.Delay(InitialDelay);

            var numberOfRows = Math.Ceiling(ActualHeight / ItemHeight);
            var last = (numberOfRows - 1) + (numberOfColumns - 1);

            for (var i = 0; i <= last; i++)
            {
                var sum = i;
                var containers = _visibleItemContainers.Where(c => c.Value.Row + c.Value.Column == sum).Select(k => k.Key);

                foreach (var container in containers)
                {
                    var containerVisual = ElementCompositionPreview.GetElementVisual(container);
                    _scaleAnimation.DelayTime = TimeSpan.FromMilliseconds(AnimationDelay * i);
                    containerVisual.CenterPoint = new Vector3((float)ItemWidth, (float)ItemHeight, 0) * 0.5f;
                    containerVisual.StartAnimation(nameof(Visual.Scale), _scaleAnimation);
                    container.Animate(null, 1.0d, nameof(Opacity), AnimationDuration, AnimationDelay * i);
                }
            }
        }

        private static CubicBezierEasingFunction EaseOutCubic(Compositor compositor) =>
            compositor.CreateCubicBezierEasingFunction(new Vector2(0.215f, 0.61f), new Vector2(0.355f, 1.0f));

        /// <summary>
        /// Calculates the width of the grid items.
        /// </summary>
        /// <param name="containerWidth">The width of the container control.</param>
        /// <returns>The calculated item width.</returns>
        protected virtual double CalculateItemWidth(double containerWidth)
        {
            var desiredWidth = double.IsNaN(DesiredWidth) ? containerWidth : DesiredWidth;

            var columns = CalculateColumns(containerWidth, desiredWidth);

            // If there's less items than there's columns, reduce the column count (if requested);
            if (Items != null && Items.Count > 0 && Items.Count < columns && StretchContentForSingleRow)
            {
                columns = Items.Count;
            }

            return (containerWidth - Padding.Left - Padding.Right) / columns - 8.0d * 2 - 2.0d;
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call
        /// ApplyTemplate. In simplest terms, this means the method is called just before a UI element displays
        /// in your app. Override this method to influence the default post-template logic of a class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            OnOneRowModeEnabledChanged(this, OneRowModeEnabled);
        }

        private void ItemsOnVectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs @event)
        {
            if (!double.IsNaN(ActualWidth))
            {
                // If the item count changes, check if more or less columns needs to be rendered,
                // in case we were having fewer items than columns.
                RecalculateLayout(ActualWidth);
            }
        }

        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            var cmd = ItemClickCommand;
            if (cmd != null)
            {
                if (cmd.CanExecute(e.ClickedItem))
                {
                    cmd.Execute(e.ClickedItem);
                }
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // If the width of the internal list view changes, check if more or less columns needs to be rendered.
            if (!e.PreviousSize.Width.Equals(e.NewSize.Width))
            {
                RecalculateLayout(e.NewSize.Width);

                if (ItemsPanelRoot is ItemsWrapGrid itemsWrapGrid)
                {
                    for (var i = itemsWrapGrid.FirstVisibleIndex; i <= itemsWrapGrid.LastVisibleIndex; i++)
                    {
                        if (ContainerFromIndex(i) is GridViewItem container)
                        {
                            container.Animate(null, 1.0d, nameof(Opacity), AnimationDuration);
                        }
                    }
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
            DetermineOneRowMode();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = false;
        }

        private void DetermineOneRowMode()
        {
            if (_isLoaded)
            {
                var itemsWrapGridPanel = ItemsPanelRoot as ItemsWrapGrid;

                if (OneRowModeEnabled)
                {
                    var b = new Binding
                    {
                        Source = this,
                        Path = new PropertyPath("ItemHeight")
                    };

                    if (itemsWrapGridPanel != null)
                    {
                        _savedOrientation = itemsWrapGridPanel.Orientation;
                        itemsWrapGridPanel.Orientation = Orientation.Vertical;
                    }

                    SetBinding(MaxHeightProperty, b);

                    _savedHorizontalScrollMode = ScrollViewer.GetHorizontalScrollMode(this);
                    _savedVerticalScrollMode = ScrollViewer.GetVerticalScrollMode(this);
                    _savedHorizontalScrollBarVisibility = ScrollViewer.GetHorizontalScrollBarVisibility(this);
                    _savedVerticalScrollBarVisibility = ScrollViewer.GetVerticalScrollBarVisibility(this);
                    _needToRestoreScrollStates = true;

                    ScrollViewer.SetVerticalScrollMode(this, ScrollMode.Disabled);
                    ScrollViewer.SetVerticalScrollBarVisibility(this, ScrollBarVisibility.Hidden);
                    ScrollViewer.SetHorizontalScrollBarVisibility(this, ScrollBarVisibility.Visible);
                    ScrollViewer.SetHorizontalScrollMode(this, ScrollMode.Enabled);
                }
                else
                {
                    ClearValue(MaxHeightProperty);

                    if (!_needToRestoreScrollStates)
                    {
                        return;
                    }

                    _needToRestoreScrollStates = false;

                    if (itemsWrapGridPanel != null)
                    {
                        itemsWrapGridPanel.Orientation = _savedOrientation;
                    }

                    ScrollViewer.SetVerticalScrollMode(this, _savedVerticalScrollMode);
                    ScrollViewer.SetVerticalScrollBarVisibility(this, _savedVerticalScrollBarVisibility);
                    ScrollViewer.SetHorizontalScrollBarVisibility(this, _savedHorizontalScrollBarVisibility);
                    ScrollViewer.SetHorizontalScrollMode(this, _savedHorizontalScrollMode);
                }
            }
        }

        private void RecalculateLayout(double containerWidth)
        {       
            // width should be the displayable width
            //containerWidth = containerWidth - Padding.Left - Padding.Top;

            if (containerWidth > 0)
            {
                var newWidth = CalculateItemWidth(containerWidth);

                if (newWidth > 0 && (double.IsNaN(ItemWidth) || Math.Abs(newWidth - ItemWidth) > 1))
                {
                    ItemWidth = newWidth;
                }
            }
        }

        private class Position
        {
            public Position(int row, int column)
            {
                Row = row;
                Column = column;
            }

            public int Row { get; set; }
            public int Column { get; set; }
        }
    }
}
