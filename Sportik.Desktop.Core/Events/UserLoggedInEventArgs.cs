using System;

namespace Sportik.Desktop.Core.Events
{
    public sealed class UserLoggedInEventArgs : EventArgs
    {
        public Guid UserId { get; }

        public string Email { get; }

        public UserLoggedInEventArgs(Guid userId, string email)
        {
            UserId = userId;
            Email = email;
        }
    }
}