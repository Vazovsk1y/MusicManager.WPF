using MusicManager.Domain.Common;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services
{
    public interface IEntityPathIsValidService<TEntity, TStorageType> 
        where TEntity : Entity 
        where TStorageType : FileSystemInfo
    {
        Result<TStorageType> IsValidPath(string path);
    }
}
