using System;

namespace Sportik.Desktop.Infrastructure.Models
{
    internal sealed class RefreshTokenCache
    {
        public string RefreshToken { get; set; }

        public Guid UserId { get; set; }
    }
}