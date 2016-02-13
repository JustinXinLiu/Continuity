using System.Numerics;
using Windows.UI.Composition;

namespace Continuity.Extensions
{
    public static partial class Extensions
    {
        public static CubicBezierEasingFunction CreateEaseInCubic(this Compositor compositor)
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.55f, 0.055f), new Vector2(0.675f, 0.19f));
        }

        public static CubicBezierEasingFunction CreateEaseOutCubic(this Compositor compositor)
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.215f, 0.61f), new Vector2(0.355f, 1.0f));
        }

        public static CubicBezierEasingFunction CreateEaseInOutCubic(this Compositor compositor)
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.645f, 0.045f), new Vector2(0.355f, 1.0f));
        }
    }
}
