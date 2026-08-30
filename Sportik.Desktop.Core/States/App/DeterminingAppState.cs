using System;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.App
{
    internal sealed class DeterminingAppState : AppState
    {
        private readonly IPersistentCacheService _persistentCacheService;
        private readonly Func<IAuthService> _authServiceFactory;

        public override ApplicationState ApplicationState => ApplicationState.Unknown;

        public DeterminingAppState(AppStatesContext context, IPersistentCacheService persistentCacheService,
            Func<IAuthService> authServiceFactory) : base(context)
        {
            _persistentCacheService = persistentCacheService;
            _authServiceFactory = authServiceFactory;
        }

        protected override void HandleEnter()
        {
            if (_persistentCacheService.TryGet(out AppRunCache cache) && !cache.LastIsOnline)
            {
                Context.Switch(Context.GuestAppState);
                return;
            }

            IAuthService authService = _authServiceFactory();

            Task.Run(async () =>
            {
                OperationResult<string> result = await authService.GetTokenAsync(ActiveCancellationToken);

                Context.Switch(result.Succeeded
                    ? Context.AuthenticatedAppState
                    : Context.LoginAppState);
            });
        }

        protected override void HandleExit() { }
    }
}
