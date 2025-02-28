using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace OTD.Core.Common
{
    public class UserContext : IUserContext
    {
        public Guid UserId { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string DisplayName { get; }

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            UserId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User Id is missing."));
            Email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
            FirstName = user.FindFirst("first_name")?.Value ?? string.Empty;
            LastName = user.FindFirst("last_name")?.Value ?? string.Empty;
            DisplayName = user.FindFirst("display_name")?.Value ?? string.Empty;
        }
    }
}
