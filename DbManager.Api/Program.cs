using AutoMapper;
using DbManager.Api.Extensions;
using DbManager.Api.Middlewares;
using DbManager.Application;
using DbManager.Infrastructure;
using DbManager.Infrastructure.Extensions.DataSeeding;
using DbManager.Infrastructure.Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddApplicationApi()
    .AddInfrastructure(builder.Configuration)
    .AddControllers().Services
    .AddApplicationAuth()
    .AddEndpointsApiExplorer()
    .AddApplicationSwagger()
    .ConfigureSwagger();

var app = builder.Build();

MigrateDatabase(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalMiddlewareErrorHander>();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

static async void MigrateDatabase(IHost host)
{
    using var scope = host.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var provider = scope.ServiceProvider;

    var mapper = provider.GetService<IMapper>();

    db.Database.Migrate();
    db.Seed();
}
