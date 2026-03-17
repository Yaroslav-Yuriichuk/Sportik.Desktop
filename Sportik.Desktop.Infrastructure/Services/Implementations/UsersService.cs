using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Infrastructure.DTOs.Users;
using Sportik.Desktop.Infrastructure.Services.Interfaces;

namespace Sportik.Desktop.Infrastructure.Services.Implementations
{
    internal sealed class UsersService : IUsersService
    {
        private readonly IApiService _apiService;
        private readonly IEventsService _eventsService;

        public UsersService(IApiService apiService, IEventsService eventsService)
        {
            _apiService = apiService;
            _eventsService = eventsService;
        }

        public async Task<OperationResult<Guid>> RegisterAsync(string email, string password, CancellationToken cancellationToken)
        {
            try
            {
                RegisterResultDto result = await _apiService.PostAsync<RegisterResultDto>(
                    "/api/Users/register",
                    new RegisterRequestDto(email, password),
                    cancellationToken: cancellationToken);

                _eventsService.RaiseEvent(new UserRegisteredEventArgs(result.UserId, result.Email));

                return OperationResult<Guid>.Success(result.UserId);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                return OperationResult<Guid>.Failure(new[] { $"An error occurred during registration: {e.Message}", });
            }
        }
    }
}