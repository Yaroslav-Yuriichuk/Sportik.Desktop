using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IInternalNotificationService
    {
        void Show(GeneralNotification generalNotification);
    }
}
