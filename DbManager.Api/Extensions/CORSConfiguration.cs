namespace DbManager.Api.Extensions;

public static class CORSConfiguration
{
    public static IServiceCollection AddApplicationApiCORS(this IServiceCollection services, string policyName = "default")
    {
        services.AddCors(builder =>
        {
            builder.AddPolicy(policyName, options =>
                options
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());

            builder.DefaultPolicyName = policyName;
        });

        return services;
    }
}
