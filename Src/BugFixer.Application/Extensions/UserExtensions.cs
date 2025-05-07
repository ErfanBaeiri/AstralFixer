using BugFixer.Domain.Entities.Account;
using System.Security.Claims;

namespace BugFixer.Application.Extensions
{
    public static class UserExtensions
    {
        public static long GetUserId(this ClaimsPrincipal user)
        {
            // This method retrieves the UserId from the ClaimsPrincipal.
            // It checks if the user is authenticated and returns the UserId as a long.
            if (user.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
                {
                    return userId;
                }
            }
            return 0; // Return 0 if the user is not authenticated or UserId is not found.
        }

        public static string GetUserDisplayName(this User user)
        {
            // This method retrieves the display name of the user.
            // It checks if the user has a first name and last name, and returns them formatted.
            // If not, it returns the username.
            if (!string.IsNullOrWhiteSpace(user.FirstName) && !string.IsNullOrWhiteSpace(user.LastName))
            {
                return $"{user.FirstName} {user.LastName}";
            }

            var email = user.Email.Split("@")[0];

            return email;

        }
    }
}
