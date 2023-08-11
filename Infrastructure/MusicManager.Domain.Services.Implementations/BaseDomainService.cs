using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Utils;
using System.Text.Json;

namespace MusicManager.Domain.Services.Implementations;

public abstract class BaseDomainService
{
    protected Result<TJsonEntityType> GetEntityInfoFromJsonFile<TJsonEntityType, TEntityType>(FileInfo fileInfo) 
        where TJsonEntityType : SerializableEntityInfo<TEntityType>
        where TEntityType : class
    {
        try
        {
            using var stream = fileInfo.OpenRead();
            var deserializeResult = JsonSerializer.Deserialize<TJsonEntityType>(stream);
            return deserializeResult is null ?
                Result.Failure<TJsonEntityType>(new Error($"Unable to deserialize {fileInfo.Name} file to {nameof(TEntityType)}."))
                :
                Result.Success(deserializeResult);
        }
        catch(Exception ex)
        {
            return Result.Failure<TJsonEntityType>(new Error(ex.Message));
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