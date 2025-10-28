using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Backend.Domain.Common;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.UI.ViewModels.Account
{
    internal sealed class AccountViewModel : ViewModel, IDisposable
    {
        private Guid _userId;

        public Guid UserId
        {
            get => _userId;
            set => SetField(ref _userId, value);
        }

        private string _email;

        public string Email
        {
            get => _email;
            set => SetField(ref _email, value);
        }

        public ReactiveRelayCommand LogOutCommand { get; }

        private IAuthService AuthService => App.ServiceProvider.GetService<IAuthService>();

        private readonly CancellationTokenSource _loadCts = new CancellationTokenSource();
        private readonly CancellationTokenSource _logOutCts = new CancellationTokenSource();

        public AccountViewModel()
        {
            LogOutCommand = new ReactiveRelayCommand(LogOut);

            _ = LoadAccountAsync(_loadCts.Token);
        }

        public void Dispose()
        {
            _loadCts.Cancel();
            _logOutCts.Cancel();
        }

        private void LogOut()
        {
            _ = LogOutAsync(_logOutCts.Token);
        }

        private async Task LoadAccountAsync(CancellationToken cancellationToken)
        {
            OperationResult<Guid> userIdResult = await AuthService.GetUserIdAsync(cancellationToken);

            if (userIdResult.Succeeded)
            {
                UserId = userIdResult.Value;
            }

            OperationResult<string> emailResult = await AuthService.GetEmailAsync(cancellationToken);

            if (emailResult.Succeeded)
            {
                Email = emailResult.Value;
            }
        }

        private async Task LogOutAsync(CancellationToken cancellationToken)
        {
            LogOutCommand.IsExecutable = false;

            OperationResult result = await AuthService.LogoutAsync(cancellationToken);
            LogOutCommand.IsExecutable = !result.Succeeded;
        }
    }
}