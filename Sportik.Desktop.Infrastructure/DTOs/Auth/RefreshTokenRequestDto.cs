namespace Sportik.Desktop.Infrastructure.DTOs.Auth
{
    internal sealed class RefreshTokenRequestDto
    {
        public string RefreshToken { get; }

        public RefreshTokenRequestDto(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }
}