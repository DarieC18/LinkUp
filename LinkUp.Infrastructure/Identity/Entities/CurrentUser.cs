using LinkUp.Application.Interfaces.Users;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace LinkUp.Infrastructure.Identity.Entities
{
    public sealed class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _http;

        public CurrentUser(IHttpContextAccessor http) => _http = http;

        public bool IsAuthenticated => _http.HttpContext?.User?.Identity?.IsAuthenticated == true;
        public string? UserId => _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        public string? UserName => _http.HttpContext?.User?.Identity?.Name;
        public string? Email => _http.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    }
}
