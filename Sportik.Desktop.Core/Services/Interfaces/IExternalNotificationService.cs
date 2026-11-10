using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IExternalNotificationService
    {
        void ShowReminder(Guid exerciseId, ReminderNotification reminderNotification);
    }
}