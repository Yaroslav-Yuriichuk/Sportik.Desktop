using System;
using Windows.UI.Xaml.Controls;

namespace Sportik.Desktop.App.Services
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
