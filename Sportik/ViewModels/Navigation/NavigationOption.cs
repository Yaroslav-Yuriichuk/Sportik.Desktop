using System;
using Windows.UI.Xaml.Controls;

namespace Sportik.ViewModels.Navigation
{
    internal sealed class NavigationOption
    {
        public string Name { get; set; }

        public Symbol Icon { get; set; }

        public Type PageType { get; set; }
    }
}
