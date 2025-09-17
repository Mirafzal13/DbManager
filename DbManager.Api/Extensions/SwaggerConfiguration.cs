namespace DbManager.Api.Extensions;

using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using DbManager.Api.Utils.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authentication.JwtBearer;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddApplicationSwagger(this IServiceCollection services)
    {
        services
            .AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "DbManager.Api",
                    Description = "An ASP.NET Core Web API for managing DbManager.Api items",
                    TermsOfService = new Uri("http://localhost:5175/")
                });

                options.DescribeAllParametersInCamelCase();
                options.SupportNonNullableReferenceTypes();
                options.UseAllOfToExtendReferenceSchemas();
                options.UseAllOfForInheritance();
                options.SchemaFilter<EnumSchemaFilter>();
                options.SchemaFilter<RequireNonNullablePropertiesSchemaFilter>();
                options.MapType<TimeSpan>(() => new OpenApiSchema
                {
                    Type = "string",
                    Example = new OpenApiString("00:00:00")
                });
                options.CustomOperationIds(e =>
                {
                    var opId = e.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null;

                    return opId;
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

        return services;
    }

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(setup =>
        {
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Description = "Enter your JWT token below.",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
            setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
            setup.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    jwtSecurityScheme, Array.Empty<string>()
                }
            });
        });
    }
}
