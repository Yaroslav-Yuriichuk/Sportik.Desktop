namespace Sportik.Desktop.Infrastructure.DTOs.Auth
{
    internal sealed class RevokeTokenRequestDto
    {
        public string RefreshToken { get; }

        public RevokeTokenRequestDto(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }
}