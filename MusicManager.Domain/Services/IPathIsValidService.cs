using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services
{
    public interface IPathIsValidService<TStorageType> 
        where TStorageType : FileSystemInfo
    {
        Result<TStorageType> IsValidPath(string path);
    }
}
