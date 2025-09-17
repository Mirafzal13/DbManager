namespace DbManager.Application;

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using DbManager.Application.Common.Behaviors;
using FluentValidation.AspNetCore;
using DbManager.Application.Services;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services
            .AddFluentValidationAutoValidation(options => options.DisableDataAnnotationsValidation = true)
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScoped<IActiveConnectionService, ActiveConnectionService>();
        services.AddScoped<IPasswordProtector, PasswordProtector>();

        services.AddMemoryCache();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SystemEventLoggingBehavior<,>));


        return services;
    }
}
