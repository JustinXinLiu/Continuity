using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Continuity.Extensions
{
    public static partial class AnimationExtensions
    {
        #region Composition

        public static void StartClipAnimation(this FrameworkElement element, ClipAnimationDirection direction, float to,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
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

            visual.Clip.StartAnimation($"{direction.ToString()}Inset", 
                compositor.CreateScalarKeyFrameAnimation(null, to, duration, delay, easing, iterationBehavior));

            if (batch != null)
            {
                batch.End();
            }
        }

        public static void StartOffsetAnimation(this UIElement element, AnimationAxis axis, float? from = null, float to = 0,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;

            var visual = element.Visual();
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            visual.StartAnimation($"Offset.{axis.ToString()}",
                compositor.CreateScalarKeyFrameAnimation(from, to, duration, delay, easing, iterationBehavior));

            if (batch != null)
            {
                batch.End();
            }
        }

        public static void StartOffsetAnimation(this UIElement element, Vector3? from = null, Vector3? to = null,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
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
                to = Vector3.Zero;
            }

            visual.StartAnimation("Offset", 
                compositor.CreateVector3KeyFrameAnimation(from, to.Value, duration, delay, easing, iterationBehavior));

            if (batch != null)
            {
                batch.End();
            }
        }

        public static void StartScaleAnimation(this UIElement element, Vector2? from = null, Vector2? to = null,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
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

            visual.StartAnimation("Scale", 
                compositor.CreateVector3KeyFrameAnimation(from, to.Value, duration, delay, easing, iterationBehavior));

            if (batch != null)
            {
                batch.End();
            }
        }

        public static void StartScaleAnimation(this UIElement element, AnimationAxis axis, float? from = null, float to = 0,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;

            var visual = element.Visual();
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            visual.StartAnimation($"Scale.{axis.ToString()}", 
                compositor.CreateScalarKeyFrameAnimation(from, to, duration, delay, easing, iterationBehavior));

            if (batch != null)
            {
                batch.End();
            }
        }

        public static void StartOpacityAnimation(this UIElement element, float? from = null, float to = 1.0f,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;

            var visual = element.Visual();
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            visual.StartAnimation("Opacity", 
                compositor.CreateScalarKeyFrameAnimation(from, to, duration, delay, easing, iterationBehavior));

            if (batch != null)
            {
                batch.End();
            }
        }

        public static void StartOffsetAnimation(this Visual visual, AnimationAxis axis, float? from = null, float to = 0,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            visual.StartAnimation($"Offset.{axis.ToString()}", 
                compositor.CreateScalarKeyFrameAnimation(from, to, duration, delay, easing, iterationBehavior));

            if (batch != null)
            {
                batch.End();
            }
        }

        public static void StartOffsetAnimation(this Visual visual, Vector3? from = null, Vector3? to = null,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            if (to == null)
            {
                to = Vector3.Zero;
            }

            visual.StartAnimation("Offset", 
                compositor.CreateVector3KeyFrameAnimation(from, to.Value, duration, delay, easing, iterationBehavior));

            if (batch != null)
            {
                batch.End();
            }
        }

        public static void StartScaleAnimation(this Visual visual, Vector2? from = null, Vector2? to = null,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;
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

            visual.StartAnimation("Scale", 
                compositor.CreateVector3KeyFrameAnimation(from, to.Value, duration, delay, easing, iterationBehavior));

            if (batch != null)
            {
                batch.End();
            }
        }

        public static void StartScaleAnimation(this Visual visual, AnimationAxis axis, float? from = null, float to = 0,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            visual.StartAnimation($"Scale.{axis.ToString()}", 
                compositor.CreateScalarKeyFrameAnimation(from, to, duration, delay, easing, iterationBehavior));

            if (batch != null)
            {
                batch.End();
            }
        }

        public static void StartOpacityAnimation(this Visual visual, float? from = null, float to = 1.0f,
            double duration = 800, int delay = 0, CompositionEasingFunction easing = null, Action completed = null,
            AnimationIterationBehavior iterationBehavior = AnimationIterationBehavior.Count)
        {
            CompositionScopedBatch batch = null;
            var compositor = visual.Compositor;

            if (completed != null)
            {
                batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) => completed();
            }

            visual.StartAnimation("Opacity", 
                compositor.CreateScalarKeyFrameAnimation(from, to, duration, delay, easing, iterationBehavior));

            if (batch != null)
            {
                batch.End();
            }
        }

        public static ScalarKeyFrameAnimation CreateScalarKeyFrameAnimation(this Compositor compositor, float? from, float to, 
            double duration, double delay, CompositionEasingFunction easing, AnimationIterationBehavior iterationBehavior)
        {
            var animation = compositor.CreateScalarKeyFrameAnimation();

            animation.Duration = TimeSpan.FromMilliseconds(duration);
            if (delay != 0) animation.DelayTime = TimeSpan.FromMilliseconds(delay);
            if (from.HasValue) animation.InsertKeyFrame(0.0f, from.Value, easing);
            animation.InsertKeyFrame(1.0f, to, easing);
            animation.IterationBehavior = iterationBehavior;

            return animation;
        }

        public static Vector3KeyFrameAnimation CreateVector3KeyFrameAnimation(this Compositor compositor, Vector2? from, Vector2 to,
            double duration, double delay, CompositionEasingFunction easing, AnimationIterationBehavior iterationBehavior)
        {
            var animation = compositor.CreateVector3KeyFrameAnimation();

            animation.Duration = TimeSpan.FromMilliseconds(duration);
            animation.DelayTime = TimeSpan.FromMilliseconds(delay);
            if (from.HasValue) animation.InsertKeyFrame(0.0f, new Vector3(from.Value, 1.0f), easing);
            animation.InsertKeyFrame(1.0f, new Vector3(to, 1.0f), easing);
            animation.IterationBehavior = iterationBehavior;

            return animation;
        }

        public static Vector3KeyFrameAnimation CreateVector3KeyFrameAnimation(this Compositor compositor, Vector3? from, Vector3 to, 
            double duration, double delay, CompositionEasingFunction easing, AnimationIterationBehavior iterationBehavior)
        {
            var animation = compositor.CreateVector3KeyFrameAnimation();

            animation.Duration = TimeSpan.FromMilliseconds(duration);
            animation.DelayTime = TimeSpan.FromMilliseconds(delay);
            if (from.HasValue) animation.InsertKeyFrame(0.0f, from.Value, easing);
            animation.InsertKeyFrame(1.0f, to, easing);
            animation.IterationBehavior = iterationBehavior;

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
