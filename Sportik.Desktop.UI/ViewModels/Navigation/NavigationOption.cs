using System;
using Microsoft.UI.Xaml.Controls;

namespace Sportik.Desktop.UI.ViewModels.Navigation
{
    internal sealed class NavigationOption
    {
        public string Name { get; set; }

        public Symbol Icon { get; set; }

        public Type PageType { get; set; }
    }
}
