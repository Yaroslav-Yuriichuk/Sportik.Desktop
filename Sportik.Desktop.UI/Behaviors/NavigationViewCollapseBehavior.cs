using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Sportik.Desktop.UI.Behaviors
{
    internal sealed class NavigationViewCollapseBehavior : IDisposable
    {
        private readonly NavigationView _navigationView;
        private readonly double _collapseThreshold;
        private readonly Window _window;

        public NavigationViewCollapseBehavior(NavigationView navigationView, double collapseThreshold)
        {
            _navigationView = navigationView;
            _collapseThreshold = collapseThreshold;
            _window = App.MainWindow;

            UpdatePaneDisplayMode(_window.Bounds.Width);
            _window.SizeChanged += Window_SizeChanged;
        }

        public void Dispose()
        {
            _window.SizeChanged -= Window_SizeChanged;
        }

        private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs args)
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
