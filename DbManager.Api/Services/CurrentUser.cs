namespace DbManager.Api.Services;

using DbManager.Application.Common;
using DbManager.Domain.Exceptions;
using DbManager.Infrastructure.Extensions;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid UserId
    {
        get
        {
            if (!Guid.TryParse(_httpContextAccessor.HttpContext?.User?.Identity?.GetSubjectId(), out var userId))
                throw new AccessDeniedException();

            return userId;
        }
    }

    public string[] Roles
    {
        get
        {
            var roles = _httpContextAccessor.HttpContext?.User?.Identity?.GetRoles()
                ?? throw new AccessDeniedException();

            return roles;
        }
    }

    public bool IsAdmin
    {
        get
        {
            var isAdmin = _httpContextAccessor.HttpContext?.User?.Identity?.HasRole("Admin") ?? false;

            return isAdmin;
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
