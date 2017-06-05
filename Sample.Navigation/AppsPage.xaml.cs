using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;
using Continuity.Extensions;
using ExpressionBuilder;
using EF = ExpressionBuilder.ExpressionFunctions;

namespace Sample.Navigation
{
    public sealed partial class AppsPage : Page
    {
        private readonly Compositor _compositor;
        private Vector3 _center;
        private float _distanceToCenter;

        public ObservableCollection<MyItem> MyItems { get; } = new ObservableCollection<MyItem>();

        public AppsPage()
        {
            InitializeComponent();

            _compositor = this.Visual().Compositor;

            Loaded += (s, e) =>
            {
                MyItems.Add(new MyItem("lorem ipsum dolor", "some desc", "ms-appx:///Assets/Photos/1.jpg"));
                MyItems.Add(new MyItem("voluptatem accusantium", "some desc", "ms-appx:///Assets/Photos/2.jpg"));
                MyItems.Add(new MyItem("ullam corporis", "some desc", "ms-appx:///Assets/Photos/3.jpg"));
                MyItems.Add(new MyItem("mollitia animi", "some desc", "ms-appx:///Assets/Photos/4.jpg"));
                MyItems.Add(new MyItem("soluta nobis est", "xxxxxxx", "ms-appx:///Assets/Photos/5.jpg"));
                MyItems.Add(new MyItem("molestiae", "xxxxxxx", "ms-appx:///Assets/Photos/6.jpg"));
                MyItems.Add(new MyItem("nisi ut aliquid ex", "xxxxxxx", "ms-appx:///Assets/Photos/7.jpg"));
                MyItems.Add(new MyItem("fugiat quo", "xxxxxxx", "ms-appx:///Assets/Photos/8.jpg"));
                MyItems.Add(new MyItem("cumque nihil", "xxxxxxx", "ms-appx:///Assets/Photos/9.jpg"));
                MyItems.Add(new MyItem("saepe eveniet", "xxxxxxx", "ms-appx:///Assets/Photos/10.jpg"));
                MyItems.Add(new MyItem("quis autem", "xxxxxxx", "ms-appx:///Assets/Photos/11.jpg"));
                MyItems.Add(new MyItem("minima veniam", "xxxxxxx", "ms-appx:///Assets/Photos/12.jpg"));
                MyItems.Add(new MyItem("fugiat nulla pariatur", "xxxxxxx", "ms-appx:///Assets/Photos/13.jpg"));
                MyItems.Add(new MyItem("consequatur", "xxxxxxx", "ms-appx:///Assets/Photos/14.jpg"));
            };
        }

        private void OnTileSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width.Equals(e.NewSize.Width)) return;

            var tileGrid = (Grid)sender;
            UpdatePerspective(tileGrid);
        }

        private void OnTilePointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var tileGrid = (Grid)sender;
            var colorElement = (Rectangle)tileGrid.FindName("ColorElement");

            // Set the CenterPoint of the backing Visual, so the rotation axis will defined from the middle.
            colorElement.Visual().CenterPoint = new Vector3(tileGrid.RenderSize.ToVector2() / 2, 0.0f);

            // Calculate distance from corner of quadrant to Center
            _center = colorElement.Visual().CenterPoint;
            var xSquared = Math.Pow(tileGrid.ActualWidth / 2, 2);
            var ySquared = Math.Pow(tileGrid.ActualHeight / 2, 2);
            _distanceToCenter = (float)Math.Sqrt(xSquared + ySquared);

            // || DEFINE THE EXPRESSION FOR THE ROTATION ANGLE ||             
            // We calculate the Rotation Angle such that it increases from 0 to 3 as the cursor position moves away from the center.
            // Combined with animating the Rotation Axis, the image is "push down" on the point at which the cursor is located.
            var pointerPositionProperties = ElementCompositionPreview.GetPointerPositionPropertySet(tileGrid);
            var pointerPosition = pointerPositionProperties.GetSpecializedReference<PointerPositionPropertySetReferenceNode>().Position;
            var angleExpressionNode = 3 * (EF.Clamp(EF.Distance(_center, pointerPosition), 0, _distanceToCenter) % _distanceToCenter / _distanceToCenter);

            var rotationAngleAnimation = _compositor.CreateScalarKeyFrameAnimation();
            rotationAngleAnimation.Duration = TimeSpan.FromMilliseconds(600);
            rotationAngleAnimation.InsertExpressionKeyFrame(0.4f, angleExpressionNode);
            rotationAngleAnimation.InsertKeyFrame(1.0f, 0.0f);

            colorElement.Visual().StartAnimation("RotationAngleInDegrees", rotationAngleAnimation);

            // || DEFINE THE EXPRESSION FOR THE ROTATION AXIS ||             
            // The RotationAxis will be defined as the axis perpendicular to vector position of the hover pointer (vector from center to hover position).
            // The axis is a vector calculated by first converting the pointer position into the coordinate space where the center point (0, 0) is in the middle.
            // The perpendicular axis is then calculated by transposing the cartesian x, y components and negating one (e.g. Vector3(-y,x,0) )
            var axisAngleExpressionNode = EF.Floor((pointerPosition.X - _center.X) * EF.Conditional(pointerPosition.X == _center.X, 0, 1));

            var rotationAxisAnimation = _compositor.CreateScalarKeyFrameAnimation();
            rotationAxisAnimation.Duration = TimeSpan.FromMilliseconds(600);
            rotationAxisAnimation.InsertExpressionKeyFrame(0.4f, axisAngleExpressionNode);
            rotationAxisAnimation.InsertKeyFrame(1.0f, 0.0f);

            colorElement.Visual().StartAnimation("RotationAxis.Y", rotationAxisAnimation);
        }

        /// <summary>
        /// Define a perspective for the image so a perceived z-distance will be shown when the image rotates.
        /// </summary>
        private void UpdatePerspective(Grid root)
        {
            var visual = ElementCompositionPreview.GetElementVisual(root);

            var size = new Vector2((float)root.ActualWidth, (float)root.ActualHeight);

            Matrix4x4 perspective = new Matrix4x4(
                1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 1.0f, -1.0f / (size.X / 2),
                0.0f, 0.0f, 0.0f, 1.0f);

            // Matrix translations are to make sure the perspective is "centered".
            visual.TransformMatrix =
                Matrix4x4.CreateTranslation(-size.X / 2, -size.Y / 2, 0f) *
                perspective *
                Matrix4x4.CreateTranslation(size.X / 2, size.Y / 2, 0f);
        }

        private void OnTilePointerExited(object sender, PointerRoutedEventArgs e)
        {

        }

        private void OnTilePointerCanceled(object sender, PointerRoutedEventArgs e)
        {

        }

        private void OnTilePointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {

        }
    }
}