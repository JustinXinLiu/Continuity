using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Continuity.Extensions
{
    public static partial class Extensions
    {
        #region Composition

        public async static Task StartClipAnimation(this FrameworkElement element, ClipAnimationDirection direction, float to,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null)
        {
            CompositionScopedBatch batch = null;

            var visual = element.Visual();
            // After we get the Visual of the View, we need to SIZE it 'cause by design the
            // Size is (0,0). Without doing this, clipping will not work.
            visual.Size = new Vector2(element.ActualWidth.ToFloat(), element.ActualHeight.ToFloat());
            var compositor = visual.Compositor;

            if (visual.Clip == null)
            {
                var clip = compositor.CreateInsetClip();
                visual.Clip = clip;
            }

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            if (delay > 0)
            {
                await Task.Delay(delay);
            }

            visual.Clip.StartAnimation($"{direction.ToString()}Inset", compositor.CreateScalarKeyFrameAnimation(to, duration, easing));

            if (batch != null)
            {
                batch.End();
            }
        }

        public async static Task StartOffsetAnimation(this UIElement element, AnimationAxis axis, float to = 0,
          double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null)
        {
            CompositionScopedBatch batch = null;

            var visual = element.Visual();
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            if (delay > 0)
            {
                await Task.Delay(delay);
            }

            visual.StartAnimation($"Offset.{axis.ToString()}", compositor.CreateScalarKeyFrameAnimation(to, duration, easing));

            if (batch != null)
            {
                batch.End();
            }
        }

        public async static Task StartOffsetAnimation(this UIElement element, Vector2? to = null,
          double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null)
        {
            CompositionScopedBatch batch = null;

            var visual = element.Visual();
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            if (to == null)
            {
                to = Vector2.Zero;
            }

            if (delay > 0)
            {
                await Task.Delay(delay);
            }

            visual.StartAnimation("Offset", compositor.CreateVector3KeyFrameAnimation(to.Value, duration, easing));

            if (batch != null)
            {
                batch.End();
            }
        }

        public async static Task StartScaleAnimation(this UIElement element, Vector2? to = null,
          double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null)
        {
            CompositionScopedBatch batch = null;

            var visual = element.Visual();
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            if (to == null)
            {
                to = Vector2.One;
            }

            if (delay > 0)
            {
                await Task.Delay(delay);
            }

            visual.StartAnimation("Scale", compositor.CreateVector3KeyFrameAnimation(to.Value, duration, easing));

            if (batch != null)
            {
                batch.End();
            }
        }

        public async static Task StartScaleAnimation(this UIElement element, AnimationAxis axis, float to = 0,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null)
        {
            CompositionScopedBatch batch = null;

            var visual = element.Visual();
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            if (delay > 0)
            {
                await Task.Delay(delay);
            }

            visual.StartAnimation($"Scale.{axis.ToString()}", compositor.CreateScalarKeyFrameAnimation(to, duration, easing));

            if (batch != null)
            {
                batch.End();
            }
        }

        public async static Task StartOpacityAnimation(this UIElement element, float to = 1.0f,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null)
        {
            CompositionScopedBatch batch = null;

            var visual = element.Visual();
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            if (delay > 0)
            {
                await Task.Delay(delay);
            }

            visual.StartAnimation("Opacity", compositor.CreateScalarKeyFrameAnimation(to, duration, easing));

            if (batch != null)
            {
                batch.End();
            }
        }

        public static ScalarKeyFrameAnimation CreateScalarKeyFrameAnimation(this Compositor compositor, float to, double duration, CompositionEasingFunction easing)
        {
            var animation = compositor.CreateScalarKeyFrameAnimation();

            animation.Duration = TimeSpan.FromMilliseconds(duration);
            animation.InsertKeyFrame(1.0f, to, easing);

            return animation;
        }

        public static Vector3KeyFrameAnimation CreateVector3KeyFrameAnimation(this Compositor compositor, Vector2 to, double duration, CompositionEasingFunction easing)
        {
            var animation = compositor.CreateVector3KeyFrameAnimation();

            animation.Duration = TimeSpan.FromMilliseconds(duration);
            animation.InsertKeyFrame(1.0f, new Vector3(to, 1.0f), easing);

            return animation;
        }

        #endregion

        #region Xaml Storyboard

        public static void Animate(this DependencyObject target, double? from, double to,
          string propertyPath, int duration = 400, int startTime = 0,
          EasingFunctionBase easing = null, Action completed = null, bool enableDependentAnimation = false)
        {
            if (easing == null)
            {
                easing = new ExponentialEase();
            }

            var db = new DoubleAnimation();
            db.EnableDependentAnimation = enableDependentAnimation;
            db.To = to;
            db.From = from;
            db.EasingFunction = easing;
            db.Duration = TimeSpan.FromMilliseconds(duration);
            Storyboard.SetTarget(db, target);
            Storyboard.SetTargetProperty(db, propertyPath);

            var sb = new Storyboard();
            sb.BeginTime = TimeSpan.FromMilliseconds(startTime);

            if (completed != null)
            {
                sb.Completed += (s, e) =>
                {
                    completed();
                };
            }

            sb.Children.Add(db);
            sb.Begin();
        }

        #endregion
    }
}
