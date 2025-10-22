namespace Sportik.Desktop.Infrastructure.DTOs.Auth
{
    internal sealed class LoginRequestDto
    {
        public string Email { get; }

        public string Password { get; }

        public LoginRequestDto(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}