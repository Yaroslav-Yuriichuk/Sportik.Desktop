using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.UI.ViewModels.Login
{
    internal sealed class LoginViewModel : ViewModel, IDisposable
    {
        private string _email;

        public string Email
        {
            get => _email;
            set => SetField(ref _email, value);
        }

        private string _password;

        public string Password
        {
            get => _password;
            set => SetField(ref _password, value);
        }

        public IReactiveCommand LoginCommand { get; }

        public IReactiveCommand RegisterCommand { get; }

        private IAuthService AuthService => App.ServiceProvider.GetService<IAuthService>();
        private IEventsService EventsService => App.ServiceProvider.GetService<IEventsService>();

        private readonly CancellationTokenSource _loginCts = new CancellationTokenSource();

        public LoginViewModel()
        {
            Email = string.Empty;
            Password = string.Empty;

            LoginCommand = new ReactiveRelayCommand(Login);
            RegisterCommand = new ReactiveRelayCommand(Register);
        }

        public void Dispose()
        {
            _loginCts.Cancel();
        }

        private void Login()
        {
            _ = LoginAsync(_loginCts.Token);
        }

        private void Register()
        {
            EventsService.RaiseEvent(new RegistrationRequestedEventArgs());
        }

        private async Task LoginAsync(CancellationToken cancellationToken)
        {
            LoginCommand.IsExecutable = false;

            OperationResult<string> result = await AuthService.LoginAsync(Email, Password, cancellationToken);
            LoginCommand.IsExecutable = !result.Succeeded;
        }
    }
}
