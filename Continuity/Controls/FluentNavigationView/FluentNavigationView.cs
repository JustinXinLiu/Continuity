using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Continuity.Extensions;

namespace Continuity.Controls
{
    public sealed class FluentNavigationView : NavigationView
    {
        private SplitView _rootSplitView;

        public FluentNavigationView()
        {
            DefaultStyleKey = typeof(FluentNavigationView);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _rootSplitView = GetTemplateChild<SplitView>("RootSplitView");
        }

        private T GetTemplateChild<T>(string name, string message = null) where T : DependencyObject
        {
            if (GetTemplateChild(name) is T child)
            {
                return child;
            }

            if (message == null)
            {
                message = $"{name} should not be null! Check the default Generic.xaml.";
            }

            throw new NullReferenceException(message);
        }
    }
}