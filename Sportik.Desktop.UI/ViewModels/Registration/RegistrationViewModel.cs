using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.UI.ViewModels.Registration
{
    internal sealed class RegistrationViewModel : ViewModel, IDisposable
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

        public IReactiveCommand RegisterCommand { get; }

        public IReactiveCommand LoginCommand { get; }

        public IReactiveCommand UseGuestModeCommand { get; }

        private IUsersService UsersService => App.ServiceProvider.GetService<IUsersService>();
        private IEventsService EventsService => App.ServiceProvider.GetService<IEventsService>();

        private readonly CancellationTokenSource _registrationCts = new CancellationTokenSource();

        public RegistrationViewModel()
        {
            Email = string.Empty;
            Password = string.Empty;

            RegisterCommand = new ReactiveRelayCommand(Register);
            LoginCommand = new ReactiveRelayCommand(Login);
            UseGuestModeCommand = new ReactiveRelayCommand(UseGuestMode);
        }

        public void Dispose()
        {
            _registrationCts.Cancel();
        }

        private void Register()
        {
            _ = RegisterAsync(_registrationCts.Token);
        }

        private void Login()
        {
            EventsService.RaiseEvent(new LoginRequestedEventArgs());
        }

        private void UseGuestMode()
        {
            EventsService.RaiseEvent(new GuestModeRequestedEventArgs());
        }

        private async Task RegisterAsync(CancellationToken cancellationToken)
        {
            RegisterCommand.IsExecutable = false;
            UseGuestModeCommand.IsExecutable = false;

            OperationResult<Guid> result = await UsersService.RegisterAsync(Email, Password, cancellationToken);

            RegisterCommand.IsExecutable = !result.Succeeded;
            UseGuestModeCommand.IsExecutable = !result.Succeeded;
        }
    }
}