using System;
using Windows.UI.Xaml.Controls;
using Sportik.Desktop.UI.Services.Interfaces;

namespace Sportik.Desktop.UI.Services.Implementations
{
    internal sealed class FrameNavigationService : INavigationService
    {
        private Frame _frame;

        public void Initialize(Frame frame)
        {
            _frame = frame;
        }

        public void Navigate(Type pageType)
        {
            _frame?.Navigate(pageType);
        }
    }
}
