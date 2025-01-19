using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sportik.Behaviors
{
    internal sealed class NavigationViewCollapseBehavior : DependencyObject
    {
        public static double GetCollapseThreshold(DependencyObject obj)
        {
            return (double)obj.GetValue(CollapseThresholdProperty);
        }

        public static void SetCollapseThreshold(DependencyObject obj, double value)
        {
            obj.SetValue(CollapseThresholdProperty, value);
        }

        public static readonly DependencyProperty CollapseThresholdProperty =
            DependencyProperty.RegisterAttached(
                "CollapseThreshold",
                typeof(double),
                typeof(NavigationViewCollapseBehavior),
                new PropertyMetadata(700, OnThresholdChanged));

        private static void OnThresholdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NavigationView navigationView)
            {
                Window.Current.SizeChanged += (s, args) =>
                {
                    double windowWidth = args.Size.Width;

                    navigationView.PaneDisplayMode = windowWidth >= GetCollapseThreshold(navigationView)
                        ? NavigationViewPaneDisplayMode.Left
                        : NavigationViewPaneDisplayMode.LeftCompact;

                    navigationView.IsPaneToggleButtonVisible = windowWidth >= GetCollapseThreshold(navigationView);
                };
            }
        }
    }
}
