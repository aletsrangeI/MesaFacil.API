namespace Interface.Persistence;

public interface IDatabaseInitializer
{
    Task InitializeAsync(CancellationToken ct = default);
}