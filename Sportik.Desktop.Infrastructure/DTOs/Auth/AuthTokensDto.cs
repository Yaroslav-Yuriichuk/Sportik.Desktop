namespace Sportik.Desktop.Infrastructure.DTOs.Auth
{
    internal sealed class AuthTokensDto
    {
        public string AccessToken { get; }

        public string RefreshToken { get; }

        public string TokenType { get; }

        public long ExpiresIn { get; }

        public AuthTokensDto(string accessToken, string refreshToken, string tokenType, long expiresIn)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            TokenType = tokenType;
            ExpiresIn = expiresIn;
        }
    }
}