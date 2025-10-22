using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Backend.Domain.Common;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.UI.Models;
using Sportik.Desktop.UI.Services.Interfaces;
using Sportik.Desktop.UI.Views.Main;

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
        private INavigationService NavigationService => App.ServiceProvider.GetService<INavigationService>();

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
        }

        private async Task LoginAsync(CancellationToken cancellationToken)
        {
            LoginCommand.IsExecutable = false;

            OperationResult<string> result = await AuthService.LoginAsync(Email, Password, cancellationToken);

            if (result.Succeeded)
            {
                NavigationService.Navigate(typeof(MainPage), NavigationScope.Main);
                return;
            }

            LoginCommand.IsExecutable = true;
        }
    }
}
