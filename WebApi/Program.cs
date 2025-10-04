using Persistence;
using MesaFacil.API.Modules.Authentication;
using MesaFacil.API.Modules.Endpoints;
using MesaFacil.API.Modules.Feature;
using MesaFacil.API.Modules.Injection;
using MesaFacil.API.Modules.Watch;
using Scalar.AspNetCore;
using UseCases;
using WatchDog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddFeature(builder.Configuration);
builder.Services.AddInjection(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddWatchDog(builder.Configuration);
builder.Services.AddOpenApi();

var app = builder.Build();

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

app.MapCatalogoEndpoints();

app.Run();

public partial class Program
{
};