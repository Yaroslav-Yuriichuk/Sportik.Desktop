using System.Collections.Generic;
using Sportik.Desktop.Core.Common.DataExchange;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Extensions;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Models.AppSettings
{
    public sealed class AutomaticDataExchangeSettings : ISettings
    {
        private readonly IPersistentCacheService _persistentCacheService;
        private readonly IEventsService _eventsService;

        public IReadOnlyList<IDataExchangeConfig> Configs => _persistentCacheService.GetOrNew<AutomaticDataExchangeCache>().Configs;

        public AutomaticDataExchangeSettings(IPersistentCacheService persistentCacheService, IEventsService eventsService)
        {
            _persistentCacheService = persistentCacheService;
            _eventsService = eventsService;
        }

        public bool TryAdd(IDataExchangeConfig config)
        {
            AutomaticDataExchangeCache cache = _persistentCacheService.GetOrNew<AutomaticDataExchangeCache>();

            if (!cache.TryAddConfig(config))
            {
                return false;
            }

            _persistentCacheService.Set(cache);
            _eventsService.RaiseEvent(new DataExchangeConfigAddedEventArgs(config));

            return true;
        }

        public bool TryRemove(IDataExchangeConfig config)
        {
            AutomaticDataExchangeCache cache = _persistentCacheService.GetOrNew<AutomaticDataExchangeCache>();

            if (!cache.TryRemoveConfig(config))
            {
                return false;
            }

            _persistentCacheService.Set(cache);
            _eventsService.RaiseEvent(new DataExchangeConfigRemovedEventArgs(config));

            return true;
        }

        public bool TryEdit(IDataExchangeConfig oldConfig, IDataExchangeConfig newConfig)
        {
            AutomaticDataExchangeCache cache = _persistentCacheService.GetOrNew<AutomaticDataExchangeCache>();

            if (!cache.TryRemoveConfig(oldConfig) || !cache.TryAddConfig(newConfig))
            {
                return false;
            }

            _persistentCacheService.Set(cache);
            _eventsService.RaiseEvent(new DataExchangeConfigEditedEventArgs(oldConfig, newConfig));

            return true;
        }
    }
}