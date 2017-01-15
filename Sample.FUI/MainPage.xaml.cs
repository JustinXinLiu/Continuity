using Continuity;
using Continuity.Extensions;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Shapes;

namespace Sample.FUI
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            Loaded += OnMainPageLoaded;
        }

        private void OnMainPageLoaded(object sender, RoutedEventArgs e)
        {
            var compositor = this.Visual().Compositor;

            // Set implicit animations for all Rectangles inside the StackPanel.
            foreach (var element in Container.Children)
            {
                var rectangle = element as Rectangle;
                rectangle?.Visual().EnableImplicitAnimation(VisualPropertyType.Offset, delay: 0.0f * Container.Children.IndexOf(rectangle));
            }

            var originalOffsetYOfMyRectangle = MyRectangle.OffsetY(Container);

            var hideOpacityAnimation = compositor.CreateScalarKeyFrameAnimation();
            hideOpacityAnimation.InsertKeyFrame(1.0f, 0.0f);
            hideOpacityAnimation.Duration = TimeSpan.FromMilliseconds(800);
            hideOpacityAnimation.Target = "Opacity";

            var hideOffsetAnimation = compositor.CreateVector3KeyFrameAnimation();
            // 24.0f here is the left Margin of MyRectangle.
            // -24.0f here is how much we want to move MyRectangle up.
            hideOffsetAnimation.InsertKeyFrame(1.0f, new Vector3(24.0f, originalOffsetYOfMyRectangle - 24.0f, 0.0f));
            hideOffsetAnimation.Duration = TimeSpan.FromMilliseconds(800);
            hideOffsetAnimation.Target = "Offset";

            var hideAnimationGroup = compositor.CreateAnimationGroup();
            hideAnimationGroup.Add(hideOpacityAnimation);
            hideAnimationGroup.Add(hideOffsetAnimation);

            ElementCompositionPreview.SetImplicitHideAnimation(MyRectangle, hideAnimationGroup);

            var showOpacityAnimation = compositor.CreateScalarKeyFrameAnimation();
            showOpacityAnimation.InsertKeyFrame(1.0f, 1.0f);
            showOpacityAnimation.Duration = TimeSpan.FromMilliseconds(800);
            showOpacityAnimation.Target = "Opacity";

            var showOffsetAnimation = compositor.CreateVector3KeyFrameAnimation();
            showOffsetAnimation.InsertKeyFrame(1.0f, new Vector3(24.0f, originalOffsetYOfMyRectangle, 0.0f));
            showOffsetAnimation.Duration = TimeSpan.FromMilliseconds(800);
            showOffsetAnimation.Target = "Offset";

            var showAnimationGroup = compositor.CreateAnimationGroup();
            showAnimationGroup.Add(showOpacityAnimation);
            showAnimationGroup.Add(showOffsetAnimation);

            ElementCompositionPreview.SetImplicitShowAnimation(MyRectangle, showAnimationGroup);
        }

        private void OnToggled(object sender, RoutedEventArgs e) =>
            MyRectangle.Visibility = Toggle.IsOn ? Visibility.Visible : Visibility.Collapsed;
    }
}
