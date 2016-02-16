using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
//using Microsoft.UI.Composition.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI.Composition;

namespace Continuity.Extensions
{
    public static partial class ImagingExtensions
    {
        //public static CompositionImageFactory CreateImageFactory(this Compositor compositor)
        //{
        //    return CompositionImageFactory.CreateCompositionImageFactory(compositor);
        //}

        //public static CompositionSurfaceBrush CreateImageBrush(this Compositor compositor, string path)
        //{
        //    var imageFactory = compositor.CreateImageFactory();
        //    var image = imageFactory.CreateImageFromUri(new Uri(path));

        //    var surfaceBrush = compositor.CreateSurfaceBrush();
        //    surfaceBrush.Surface = image.Surface;
        //    return surfaceBrush;
        //}

        //public static CompositionEffectBrush CreateCompositeImageBrush(this Compositor compositor, List<Tuple<string, string>> images)
        //{
        //    var effect = new CompositeEffect { Mode = CanvasComposite.DestinationIn };

        //    foreach (var image in images)
        //    {
        //        var param = new CompositionEffectSourceParameter(image.Item1);
        //        effect.Sources.Add(param);
        //    }

        //    var effectFactory = compositor.CreateEffectFactory(effect);
        //    var brush = effectFactory.CreateBrush();

        //    foreach (var image in images)
        //    {
        //        brush.SetSourceParameter(image.Item1, compositor.CreateImageBrush(image.Item2));
        //    }

        //    return brush;
        //}
    }
}
