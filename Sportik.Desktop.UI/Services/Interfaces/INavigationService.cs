using System;
using Sportik.Desktop.UI.Models;

namespace Sportik.Desktop.UI.Services.Interfaces
{
    internal interface INavigationService
    {
        void Navigate(Type pageType, NavigationScope scope);
    }
}
