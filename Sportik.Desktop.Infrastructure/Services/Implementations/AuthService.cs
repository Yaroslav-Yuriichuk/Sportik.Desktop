using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Backend.Domain.Common;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Infrastructure.DTOs.Auth;
using Sportik.Desktop.Infrastructure.Helpers;
using Sportik.Desktop.Infrastructure.Models;
using Sportik.Desktop.Infrastructure.Services.Interfaces;

namespace Sportik.Desktop.Infrastructure.Services.Implementations
{
    internal sealed class AuthService : IAuthService
    {
        private readonly IApiService _apiService;
        private readonly ISecureCacheService _secureCacheService;
        private readonly IRuntimeCacheService _runtimeCacheService;

        public AuthService(IApiService apiService, ISecureCacheService secureCacheService, IRuntimeCacheService runtimeCacheService)
        {
            _apiService = apiService;
            _secureCacheService = secureCacheService;
            _runtimeCacheService = runtimeCacheService;
        }

        public async Task<OperationResult<string>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            try
            {
                AuthTokensDto authTokens = await _apiService.PostAsync<AuthTokensDto>(
                    "api/Auth/login",
                    new LoginRequestDto(email, password),
                    cancellationToken: cancellationToken);

                string accessToken = authTokens.AccessToken;

                if (!JwtTokenHelper.TryGetUserId(accessToken, out Guid userId))
                {
                    return OperationResult<string>.Failure(new[] { "Login failed: Unable to parse access token.", });
                }

                AccessTokenCache accessTokenCache = new AccessTokenCache
                {
                    AccessToken = authTokens.AccessToken,
                };

                RefreshTokenCache refreshTokenCache = new RefreshTokenCache
                {
                    RefreshToken = authTokens.RefreshToken,
                    UserId = userId,
                };

                _runtimeCacheService.Set(accessTokenCache);
                _secureCacheService.Set(refreshTokenCache);

                return OperationResult<string>.Success(authTokens.AccessToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                return OperationResult<string>.Failure(new[] { $"Login failed: {e.Message}", });
            }
        }

        public async Task<OperationResult<string>> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            if (_runtimeCacheService.TryGet(out AccessTokenCache accessTokenCache) &&
                JwtTokenHelper.TryGetExpiration(accessTokenCache.AccessToken, out DateTimeOffset expiresAt)
                && expiresAt > DateTimeOffset.UtcNow.AddMinutes(1))
            {
                return OperationResult<string>.Success(accessTokenCache.AccessToken);
            }

            if (!_secureCacheService.TryGet(out RefreshTokenCache refreshTokenCache))
            {
                return OperationResult<string>.Failure(new[] { "No refresh token available." });
            }

            try
            {
                AuthTokensDto authTokens = await _apiService.PostAsync<AuthTokensDto>(
                    "api/Auth/refresh",
                    new RefreshTokenRequestDto(refreshTokenCache.RefreshToken),
                    cancellationToken: cancellationToken);

                string accessToken = authTokens.AccessToken;

                if (!JwtTokenHelper.TryGetUserId(accessToken, out Guid userId))
                {
                    return OperationResult<string>.Failure(new[] { "Token refresh failed: Unable to parse access token.", });
                }

                accessTokenCache = new AccessTokenCache
                {
                    AccessToken = authTokens.AccessToken,
                };

                refreshTokenCache = new RefreshTokenCache
                {
                    RefreshToken = authTokens.RefreshToken,
                    UserId = userId,
                };

                _runtimeCacheService.Set(accessTokenCache);
                _secureCacheService.Set(refreshTokenCache);

                return OperationResult<string>.Success(authTokens.AccessToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                return OperationResult<string>.Failure(new[] { $"Token refresh failed: {e.Message}", });
            }
        }

        public async Task<OperationResult> LogoutAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _runtimeCacheService.Remove<AccessTokenCache>();
                _secureCacheService.Remove<RefreshTokenCache>();

                // TODO: Revoke refresh token on the server.
                await Task.CompletedTask;

                return OperationResult.Success();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                return OperationResult.Failure(new[] { $"Logout failed: {e.Message}", });
            }
        }
    }
}