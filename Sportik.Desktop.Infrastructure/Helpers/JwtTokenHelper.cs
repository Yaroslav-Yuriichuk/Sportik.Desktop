using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Sportik.Desktop.Infrastructure.Helpers
{
    internal sealed class JwtTokenHelper
    {
        public static bool TryGetUserId(string accessToken, out Guid userId)
        {
            userId = Guid.Empty;

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return false;
            }

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(accessToken))
            {
                return false;
            }

            JwtSecurityToken jwtToken = handler.ReadJwtToken(accessToken);
            Claim subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);

            if (subClaim == null || !Guid.TryParse(subClaim.Value, out userId))
            {
                return false;
            }

            return true;
        }

        public static bool TryGetExpiration(string accessToken, out DateTimeOffset expiresAt)
        {
            expiresAt = DateTimeOffset.MinValue;

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return false;
            }

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(accessToken))
            {
                return false;
            }

            JwtSecurityToken jwtToken = handler.ReadJwtToken(accessToken);

            if (jwtToken.ValidTo == DateTime.MinValue)
            {
                return false;
            }

            expiresAt = new DateTimeOffset(jwtToken.ValidTo, TimeSpan.Zero);
            return true;
        }

        public static bool TryGetEmail(string accessToken, out string email)
        {
            email = string.Empty;

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return false;
            }

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(accessToken))
            {
                return false;
            }

            JwtSecurityToken jwtToken = handler.ReadJwtToken(accessToken);
            Claim emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email);

            if (emailClaim == null)
            {
                return false;
            }

            email = emailClaim.Value;
            return true;
        }
    }
}