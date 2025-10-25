using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
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

        private IEventsService EventsService => App.ServiceProvider.GetService<IEventsService>();

        private readonly CancellationTokenSource _loginCts = new CancellationTokenSource();

        public RegistrationViewModel()
        {
            Email = string.Empty;
            Password = string.Empty;

            RegisterCommand = new ReactiveRelayCommand(Register);
            LoginCommand = new ReactiveRelayCommand(Login);
        }

        public void Dispose()
        {
            _loginCts.Cancel();
        }

        private void Register()
        {
        }

        private void Login()
        {
            EventsService.RaiseEvent(new LoginRequestedEventArgs());
        }
    }
}