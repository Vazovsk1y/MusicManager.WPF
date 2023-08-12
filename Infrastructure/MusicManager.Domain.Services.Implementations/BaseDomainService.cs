using MusicManager.Domain.Common;
using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Utils;
using System.Text.Json;

namespace MusicManager.Domain.Services.Implementations;

public abstract class BaseDomainService
{
    protected Result<TSerializable> GetEntityInfoFromJsonFile<TSerializable, TEntity>(FileInfo fileInfo) 
        where TSerializable : SerializableEntity<TEntity>
        where TEntity : class, IAggregateRoot
    {
        try
        {
            using var stream = fileInfo.OpenRead();
            var deserializeResult = JsonSerializer.Deserialize<TSerializable>(stream);
            return deserializeResult is null ?
                Result.Failure<TSerializable>(new Error($"Unable to deserialize {fileInfo.Name} file to {typeof(TEntity).Name}."))
                :
                Result.Success(deserializeResult);
        }
        catch(Exception ex)
        {
            return Result.Failure<TSerializable>(new Error(ex.Message));
        }
    }

    protected Result<TFileSystem> IsAbleToMoveNext<TFileSystem>(string songWriterPath)
        where TFileSystem : FileSystemInfo
    {
        if (!PathValidator.IsValid(songWriterPath))
        {
            return Result.Failure<TFileSystem>(DomainServicesErrors.PassedDirectoryPathIsInvalid(songWriterPath));
        }

        var fileSystemInfo = (TFileSystem)Activator.CreateInstance(typeof(TFileSystem), songWriterPath);
        if (!fileSystemInfo.Exists)
        {
            return Result.Failure<TFileSystem>(DomainServicesErrors.PassedDirectoryIsNotExists(songWriterPath));
        }

        return fileSystemInfo;
    }
}