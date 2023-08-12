using MusicManager.Domain.Common;

namespace MusicManager.Domain.Services;

public abstract class SerializableEntity<TEntity> where TEntity : class, IAggregateRoot
{
    public static string FileName => $"{typeof(TEntity).Name}{InfoJsonKeyWord}";

    public const string InfoJsonKeyWord = "Info.json";
}
