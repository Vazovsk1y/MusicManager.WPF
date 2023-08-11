namespace MusicManager.Domain.Services;

public abstract class SerializableEntityInfo<TEntity> where TEntity : class
{
    public static string FileName => $"{nameof(TEntity)}{InfoJsonPrefix}";

    public const string InfoJsonPrefix = "info.json";
}
