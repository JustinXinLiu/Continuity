using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Sample.Main.Common
{
    public static class Extensions
    {
        public static void AddRange<T>(this ObservableCollection<T> newCollection, IList<T> oldCollection, int durationInMs = 50)
        {
            var observable = Observable.Generate(0, i => i <= oldCollection.Count - 1, i => ++i, i => oldCollection[i], i => TimeSpan.FromMilliseconds(durationInMs));
            observable.ObserveOnDispatcher().Subscribe((i) => newCollection.Add(i));
        }

        public static ScrollViewer GetScrollViewer(this DependencyObject element)
        {
            if (element is ScrollViewer)
            {
                return (ScrollViewer)element;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }

            return null;
        }


    }
}
