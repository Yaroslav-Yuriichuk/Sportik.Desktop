using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Sportik.Desktop.UI.Helpers
{
    internal static class UIThreadHelper
    {
        public static async Task RunOnUIThreadAsync(Action action, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            CoreDispatcher dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

            if (dispatcher.HasThreadAccess)
            {
                action();
                return;
            }

            bool wasCanceled = false;

            await dispatcher.RunAsync(
                priority, () =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        wasCanceled = true;
                        return;
                    }

                    action();
                });

            if (wasCanceled)
            {
                throw new OperationCanceledException(cancellationToken);
            }
        }
    }
}
