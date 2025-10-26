namespace Sportik.Desktop.Infrastructure.DTOs.Users
{
    internal sealed class RegisterRequestDto
    {
        public string Email { get; }

        public string Password { get; }

        public RegisterRequestDto(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}