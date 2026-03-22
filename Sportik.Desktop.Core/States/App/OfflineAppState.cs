using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.App
{
    internal sealed class OfflineAppState : AppState
    {
        private readonly IRuntimeCacheService _runtimeCacheService;

        public override ApplicationState ApplicationState => ApplicationState.Offline;

        public OfflineAppState(AppStatesContext context, IRuntimeCacheService runtimeCacheService) : base(context)
        {
            _runtimeCacheService = runtimeCacheService;
        }

        protected override void HandleEnter()
        {
            _runtimeCacheService.Set(new AppModeCache
            {
                IsOffline = true,
            });


        }

        protected override void HandleExit()
        {
            _runtimeCacheService.Remove<AppModeCache>();
        }
    }
}