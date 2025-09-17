using DbManager.Api.Services;
using DbManager.Application.Common;
using System.Reflection;

namespace DbManager.Api.Extensions;


internal static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplicationApi(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddApplicationApiCORS();

        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddTransient<IAuthService, AuthService>();

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}
