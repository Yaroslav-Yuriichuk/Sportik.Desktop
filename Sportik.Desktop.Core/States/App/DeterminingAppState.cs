using System;
using System.Threading.Tasks;
using Sportik.Backend.Domain.Common;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.App
{
    internal sealed class DeterminingAppState : AppState
    {
        private readonly Func<IAuthService> _authServiceFactory;

        public override ApplicationState ApplicationState => ApplicationState.Unknown;

        public DeterminingAppState(AppStatesContext context, Func<IAuthService> authServiceFactory) : base(context)
        {
            _authServiceFactory = authServiceFactory;
        }

        protected override void HandleEnter()
        {
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
