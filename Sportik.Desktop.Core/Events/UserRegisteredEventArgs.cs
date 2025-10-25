using System;

namespace Sportik.Desktop.Core.Events
{
    internal sealed class UserRegisteredEventArgs : EventArgs
    {
        public Guid UserId { get; }

        public string Email { get; }

        public UserRegisteredEventArgs(Guid userId, string email)
        {
            UserId = userId;
            Email = email;
        }
    }
}