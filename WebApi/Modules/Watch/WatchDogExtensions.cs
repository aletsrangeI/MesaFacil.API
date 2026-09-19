using WatchDog;

namespace MesaFacil.API.Modules.Watch;

public static class WatchDogExtensions
{
    public static IServiceCollection AddWatchDog(this IServiceCollection services, IConfiguration configuration)
    {
        var rawConn = new[]
        {
            Environment.GetEnvironmentVariable("ConnectionStrings__mesafacil_db"),
            configuration.GetConnectionString("mesafacil_db"),
            configuration["ConnectionStrings:mesafacil_db"],
            configuration["ConnectionStrings__mesafacil_db"],
            configuration["mesafacil_db"]
        }.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));

        var connString = Persistence.ConfigureServices.SanitizeConnectionString(rawConn);

        services.AddWatchDogServices(opt =>
        {
            opt.SetExternalDbConnString = connString;
            opt.DbDriverOption = WatchDog.src.Enums.WatchDogDbDriverEnum.PostgreSql;
            opt.IsAutoClear = true;
            opt.ClearTimeSchedule = WatchDog.src.Enums.WatchDogAutoClearScheduleEnum.Monthly;
        });
        return services;
    }
}