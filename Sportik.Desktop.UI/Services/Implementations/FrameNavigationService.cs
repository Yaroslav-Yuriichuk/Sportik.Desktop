using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Sportik.Desktop.UI.Models;
using Sportik.Desktop.UI.Services.Interfaces;

namespace Sportik.Desktop.UI.Services.Implementations
{
    internal sealed class FrameNavigationService : INavigationService
    {
        private readonly Dictionary<NavigationScope, Frame> _frames = new Dictionary<NavigationScope, Frame>();

        public void SetFrame(Frame frame, NavigationScope scope)
        {
            _frames[scope] = frame;
        }

        public void Navigate(Type pageType, NavigationScope scope)
        {
            if (_frames.TryGetValue(scope, out Frame frame))
            {
                frame.Navigate(pageType);
            }
        }
    }
}
