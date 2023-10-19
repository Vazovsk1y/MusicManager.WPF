using Microsoft.Extensions.Logging;
using MusicManager.Domain.Enums;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services.Storage;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;
using System.Collections.Concurrent;

namespace MusicManager.Domain.Services.Implementations;

public partial class FolderToCompilationService : 
    BaseDiscDomainService,
    IFolderToCompilationService
{
    #region --Fields--

    private readonly ConcurrentDictionary<string, Compilation> _cache = new();

	#endregion

	#region --Properties--



	#endregion

	#region --Constructors--

	public FolderToCompilationService(IRoot userConfig, ILogger<FolderToCompilationService> logger) : base(userConfig, logger)
	{
	}

	#endregion

	#region --Methods--

	public Task<Result<Compilation>> GetEntityAsync(string compilationPath, SongwriterId parentId)
    {
        var isAbleToMoveNextResult = IsAbleToParse<DirectoryInfo>(compilationPath);
        if (isAbleToMoveNextResult.IsFailure)
        {
            return Task.FromResult(Result.Failure<Compilation>(isAbleToMoveNextResult.Error));
        }

        if (_cache.TryGetValue(compilationPath, out var value))
        {
            return Task.FromResult(Result.Success(value));
        }

        var directoryInfo = isAbleToMoveNextResult.Value;
        if (directoryInfo.EnumerateFiles().FirstOrDefault(e => e.Name == CompilationEntityJson.FileName) is FileInfo fileInfo)
        {
            var entityJsonResult = GetEntityInfoFromJsonFile<CompilationEntityJson, Compilation>(fileInfo);
            if (entityJsonResult.IsSuccess)
            {
                var entityResult = entityJsonResult.Value.ToEntity(parentId, compilationPath.GetRelational(_root));
                if (entityResult.IsFailure)
                {
                    Task.FromResult(Result.Failure<Compilation>(entityResult.Error));
                }

                // add to cache parsed from json entity
                _cache[compilationPath] = entityResult.Value;
                return Task.FromResult(Result.Success(entityResult.Value));
            }

            return Task.FromResult(Result.Failure<Compilation>(entityJsonResult.Error));
        }

        var compilationCreationResult = directoryInfo.Name.Contains(DiscType.Bootleg.Value) ?
            CreateBootLeg(directoryInfo, parentId) : CreateSimpleDisc(directoryInfo, parentId);

        if (compilationCreationResult.IsSuccess)
        {
            _cache[compilationPath] = compilationCreationResult.Value;
            return Task.FromResult(Result.Success(compilationCreationResult.Value));
        }

        return Task.FromResult(Result.Failure<Compilation>(compilationCreationResult.Error));
    }

    private Result<Compilation> CreateBootLeg(DirectoryInfo discDirectoryInfo, SongwriterId parent)
    {
        var discCreationResult = Compilation.Create(
            parent,
            DiscType.Bootleg,
            discDirectoryInfo.Name,
            discDirectoryInfo.FullName.GetRelational(_root));

        return discCreationResult;
    }

    private Result<Compilation> CreateSimpleDisc(DirectoryInfo discDirectoryInfo, SongwriterId parent)
    {
        var gettingComponentsResult = GetDiscComponentsFromFolderName(discDirectoryInfo.Name);

        if (gettingComponentsResult.IsFailure)
        {
            return Result.Failure<Compilation>(gettingComponentsResult.Error);
        }

        var (type, identifier, prodCountry, prodYear) = gettingComponentsResult.Value;
        if (prodCountry is null)
        {
            return Compilation.Create(
                parent,
                type,
                identifier,
                discDirectoryInfo.FullName.GetRelational(_root)
                );
        }

        var creationDiscResult = Compilation.Create(
            parent,
            type,
            identifier,
            discDirectoryInfo.FullName.GetRelational(_root),
            prodYear,
            prodCountry);

        return creationDiscResult;
    }

    #endregion
}
