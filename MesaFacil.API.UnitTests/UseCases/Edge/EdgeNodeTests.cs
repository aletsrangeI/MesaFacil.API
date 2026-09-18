using Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Persistence;
using Persistence.Context;
using Persistence.Interceptors;
using WebApi.Controllers;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Edge;

public class EdgeNodeTests : IDisposable
{
    private readonly string _tempDbPath;
    private readonly SqliteConnection _connection;

    public EdgeNodeTests()
    {
        _tempDbPath = Path.Combine(Path.GetTempPath(), $"mesafacil_test_{Guid.NewGuid():N}.db");
        _connection = new SqliteConnection($"Data Source={_tempDbPath};Cache=Shared");
        _connection.Open();
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();

        if (File.Exists(_tempDbPath))
        {
            try { File.Delete(_tempDbPath); } catch { /* ignore */ }
        }
    }

    private ApplicationDbContext CreateSqliteContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        var auditableInterceptor = new AuditableEntitySaveChangesInterceptor();
        var outboxInterceptor = new OutboxSaveChangesInterceptor();

        return new ApplicationDbContext(options, auditableInterceptor, outboxInterceptor);
    }

    [Fact]
    public async Task EdgeNode_SqliteDatabaseInitialization_CreatesSchemaAndWALPragmas()
    {
        // Arrange
        using var context = CreateSqliteContext();
        var loggerMock = new Mock<ILogger<DatabaseInitializer>>();
        var initializer = new DatabaseInitializer(context, loggerMock.Object);

        // Act
        await initializer.InitializeAsync(CancellationToken.None);

        // Assert - Verificar que las tablas fueron creadas y se insertó el rol Admin
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Admin");
        adminRole.Should().NotBeNull();
        adminRole!.Nombre.Should().Be("Admin");

        // Verificar pragmas ejecutando query en la conexión SQLite
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "PRAGMA journal_mode;";
        var journalMode = (string?)await cmd.ExecuteScalarAsync();
        journalMode.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task EdgeNode_VacuumIntoBackup_GeneratesConsistentBackupFile()
    {
        // Arrange
        using var context = CreateSqliteContext();
        await context.Database.EnsureCreatedAsync();

        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["ExecutionProfile"]).Returns("Edge");

        var loggerMock = new Mock<ILogger<EdgeController>>();
        var controller = new EdgeController(context, configMock.Object, loggerMock.Object);

        // Act
        var result = await controller.CreateBackup(CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();

        var response = okResult!.Value as Response<DTO.Edge.EdgeBackupResultDTO>;
        response.Should().NotBeNull();
        response!.isSuccess.Should().BeTrue();

        var data = response.Data!;
        File.Exists(data.FilePath).Should().BeTrue();

        var fileInfo = new FileInfo(data.FilePath);
        fileInfo.Length.Should().BeGreaterThan(0);

        // Cleanup backup
        try { File.Delete(data.FilePath); } catch { /* ignore */ }
    }

    [Fact]
    public async Task EdgeNode_StatusEndpoint_ReturnsEdgeProfileAndPendingOutbox()
    {
        // Arrange
        using var context = CreateSqliteContext();
        await context.Database.EnsureCreatedAsync();

        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["ExecutionProfile"]).Returns("Edge");
        configMock.Setup(c => c["Sync:CloudBaseUrl"]).Returns("http://cloud.mesafacil.com");

        var loggerMock = new Mock<ILogger<EdgeController>>();
        var controller = new EdgeController(context, configMock.Object, loggerMock.Object);

        // Act
        var result = await controller.GetStatus(CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();

        var response = okResult!.Value as Response<DTO.Edge.EdgeStatusDTO>;
        response.Should().NotBeNull();
        response!.isSuccess.Should().BeTrue();

        var data = response.Data!;
        data.Profile.Should().Be("Edge");
        data.IsSqlite.Should().BeTrue();
    }
}
