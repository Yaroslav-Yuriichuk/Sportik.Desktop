using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;

namespace Sportik.Desktop.UI.Helpers
{
    internal static class UIThreadHelper
    {
        public static Task RunOnUIThreadAsync(Action action, DispatcherQueuePriority priority = DispatcherQueuePriority.Normal, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            DispatcherQueue dispatcherQueue = App.MainWindow.DispatcherQueue;

            if (dispatcherQueue.HasThreadAccess)
            {
                action();
                return Task.CompletedTask;
            }

            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

            bool enqueued = dispatcherQueue.TryEnqueue(priority, () =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    taskCompletionSource.TrySetCanceled(cancellationToken);
                    return;
                }

                try
                {
                    action();
                    taskCompletionSource.TrySetResult(true);
                }
                catch (Exception exception)
                {
                    taskCompletionSource.TrySetException(exception);
                }
            });

            if (!enqueued)
            {
                taskCompletionSource.TrySetResult(false);
            }

            return taskCompletionSource.Task;
        }
    }
}
