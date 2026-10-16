using System;
using Sportik.Desktop.Core.Models.AppSettings;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Services.Implementations
{
    internal sealed class AppSettingsService : IAppSettingsService
    {
        private readonly Func<Type, ISettings> _settingsFactory;

        public AppSettingsService(Func<Type, ISettings> settingsFactory)
        {
            _settingsFactory = settingsFactory;
        }

        public T GetSettings<T>() where T : ISettings
        {
            return (T)_settingsFactory(typeof(T));
        }
    }
}