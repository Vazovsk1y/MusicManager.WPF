using Microsoft.Extensions.Logging;
using MusicManager.Domain.Common;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Services.Storage;
using MusicManager.Domain.Shared;
using System.Text.Json;

namespace MusicManager.Domain.Services.Implementations;

public abstract class BaseDomainService
{
    protected readonly IRoot _root;
    private readonly ILogger<BaseDomainService> _logger;

	protected BaseDomainService(IRoot root, ILogger<BaseDomainService> logger)
	{
		_root = root;
		_logger = logger;
	}

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
            _logger.LogError(ex, "Something go wrong while entity json file serialized.");
            return Result.Failure<TSerializable>(new Error(ex.Message));
        }
    }

    protected Result<TFileSystem> IsAbleToParse<TFileSystem>(string entityPath)
        where TFileSystem : FileSystemInfo
    {
        if (!_root.IsStoresIn(entityPath))
        {
            return Result.Failure<TFileSystem>(new Error($"Entity must be stored in root folder {_root.Path}."));
        }

        var fileSystemInfo = (TFileSystem)Activator.CreateInstance(typeof(TFileSystem), entityPath)!;
        if (!fileSystemInfo.Exists)
        {
            return Result.Failure<TFileSystem>(DomainServicesErrors.PassedDirectoryIsNotExists(entityPath));
        }

        return fileSystemInfo;
    }
}
