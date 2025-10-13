using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sportik.Desktop.UI.Behaviors
{
    internal sealed class NavigationViewCollapseBehavior : IDisposable
    {
        private readonly NavigationView _navigationView;
        private readonly double _collapseThreshold;

        public NavigationViewCollapseBehavior(NavigationView navigationView, double collapseThreshold)
        {
            _navigationView = navigationView;
            _collapseThreshold = collapseThreshold;

            UpdatePaneDisplayMode(Window.Current.Bounds.Width);
            Window.Current.SizeChanged += UpdatePaneDisplayMode;
        }

        public void Dispose()
        {
            Window.Current.SizeChanged -= UpdatePaneDisplayMode;
        }

        private void UpdatePaneDisplayMode(object sender, WindowSizeChangedEventArgs args)
        {
            double windowWidth = args.Size.Width;
            UpdatePaneDisplayMode(windowWidth);
        }

        private void UpdatePaneDisplayMode(double windowWidth)
        {
            _navigationView.PaneDisplayMode = windowWidth >= _collapseThreshold
                ? NavigationViewPaneDisplayMode.Left
                : NavigationViewPaneDisplayMode.LeftCompact;

            _navigationView.IsPaneToggleButtonVisible = windowWidth >= _collapseThreshold;
        }
    }
}
