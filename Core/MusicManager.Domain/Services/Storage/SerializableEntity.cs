using MusicManager.Domain.Common;

namespace MusicManager.Domain.Services.Storage;

public abstract class SerializableEntity<TEntity> where TEntity : class, IAggregateRoot
{
    public static string FileName => $"{typeof(TEntity).Name}Info.json";
}
