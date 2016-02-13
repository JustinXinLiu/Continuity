namespace Continuity
{
    public enum TransitionDirection
    {
        TopToBottom,
        BottomToTop,
        LeftToRight,
        RightToLeft
    }

    public enum ClipAnimationDirection
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public enum AnimationAxis
    {
        X,
        Y,
        Z
    }

    public enum AnimationType
    {
        KeyFrame,
        Expression
    }

    public enum FlickDirection
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    public enum ViewState
    {
        Empty,
        Small,
        Big,
        Full
    }
}
