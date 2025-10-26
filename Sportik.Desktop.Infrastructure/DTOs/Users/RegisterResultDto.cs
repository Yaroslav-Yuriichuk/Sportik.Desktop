using System;

namespace Sportik.Desktop.Infrastructure.DTOs.Users
{
    internal sealed class RegisterResultDto
    {
        public Guid UserId { get; }

        public string Email { get; }

        public RegisterResultDto(Guid userId, string email)
        {
            UserId = userId;
            Email = email;
        }
    }
}