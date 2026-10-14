using Sportik.Desktop.Core.Models.AppSettings;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IAppSettingsService
    {
        T GetSettings<T>() where T : ISettings;
    }
}