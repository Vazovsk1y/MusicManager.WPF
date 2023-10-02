namespace MusicManager.Repositories.Data;

public interface IDbInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
