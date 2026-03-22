using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
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

        private bool _isLoggedIn;

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set => SetField(ref _isLoggedIn, value);
        }

        public ReactiveRelayCommand LogOutCommand { get; }
        public ReactiveRelayCommand LogInCommand { get; }

        private IAuthService AuthService => App.ServiceProvider.GetRequiredService<IAuthService>();
        private IRuntimeCacheService RuntimeCacheService => App.ServiceProvider.GetRequiredService<IRuntimeCacheService>();
        private IEventsService EventsService => App.ServiceProvider.GetRequiredService<IEventsService>();

        private readonly CancellationTokenSource _loadCts = new CancellationTokenSource();
        private readonly CancellationTokenSource _logoutCts = new CancellationTokenSource();

        public AccountViewModel()
        {
            bool isLoggedIn = false;

            if (RuntimeCacheService.TryGet(out AppModeCache appModeCache))
            {
                isLoggedIn = !appModeCache.IsOffline;
            }

            IsLoggedIn = isLoggedIn;

            LogOutCommand = new ReactiveRelayCommand(LogOut, isLoggedIn);
            LogInCommand = new ReactiveRelayCommand(LogIn, !isLoggedIn);

            _ = LoadAccountAsync(_loadCts.Token);
        }

        public void Dispose()
        {
            _loadCts.Cancel();
            _logoutCts.Cancel();
        }

        private void LogOut()
        {
            _ = LogOutAsync(_logoutCts.Token);
        }

        private void LogIn()
        {
            LogInCommand.IsExecutable = false;
            EventsService.RaiseEvent(new LoginRequestedEventArgs());
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