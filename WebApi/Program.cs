using Interface.Persistence;
using Interface.UseCases;
using Persistence;
using MesaFacil.API.Modules.Authentication;
using MesaFacil.API.Modules.Endpoints;
using MesaFacil.API.Modules.Feature;
using MesaFacil.API.Modules.Injection;
using MesaFacil.API.Modules.Watch;
using Persistence.Security;
using Scalar.AspNetCore;
using UseCases;
using WatchDog;
using JwtOptions = MesaFacil.API.Modules.Authentication.JwtOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddFeature(builder.Configuration);
builder.Services.AddInjection(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddWatchDog(builder.Configuration);
builder.Services.AddOpenApi();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    await initializer.InitializeAsync();
}

app.UseDeveloperExceptionPage();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("MesaFacil API Reference")
        .WithTheme(ScalarTheme.Alternate)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});


if (app.Environment.IsDevelopment())
{
    //
}

app.UseWatchDogExceptionLogger();
app.UseHttpsRedirection();
app.UseCors("policyMesaFacil");
app.UseAuthentication();
app.UseAuthorization();

app.UseWatchDog(conf =>
{
    conf.WatchPageUsername = builder.Configuration["WatchDog:WatchPageUsername"];
    conf.WatchPagePassword = builder.Configuration["WatchDog:WatchPagePassword"];
});

app.MapControllers();

app.Run();

public partial class Program
{
};