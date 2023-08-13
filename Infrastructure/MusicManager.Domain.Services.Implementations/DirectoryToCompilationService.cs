using MusicManager.Domain.Enums;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using System.Collections.Concurrent;

namespace MusicManager.Domain.Services.Implementations;

public partial class DirectoryToCompilationService : 
    BaseDiscDomainService,
    IPathToCompilationService
{
    #region --Fields--

    private readonly ConcurrentDictionary<string, Compilation> _cache = new();

    #endregion

    #region --Properties--



    #endregion

    #region --Constructors--



    #endregion

    #region --Methods--

    public Task<Result<Compilation>> GetEntityAsync(string compilationPath, SongwriterId parent)
    {
        var isAbleToMoveNextResult = IsAbleToMoveNext<DirectoryInfo>(compilationPath);
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
            return entityJsonResult.IsFailure ?
                Task.FromResult(Result.Failure<Compilation>(entityJsonResult.Error))
                :
                Task.FromResult(entityJsonResult.Value.ToEntity(parent, compilationPath));
        }

        var compilationCreationResult = directoryInfo.Name.Contains(DiscType.Bootleg.ToString()) ?
            CreateBootLeg(directoryInfo, parent) : CreateSimpleDisc(directoryInfo, parent);

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
            discDirectoryInfo.FullName);

        return discCreationResult;
    }

    private Result<Compilation> CreateSimpleDisc(DirectoryInfo discDirectoryInfo, SongwriterId parent)
    {
        var gettingComponentsResult = GetDiscComponentsFromDirectoryName(discDirectoryInfo.Name);

        if (gettingComponentsResult.IsFailure)
        {
            return Result.Failure<Compilation>(gettingComponentsResult.Error);
        }

        var (type, identificator, prodCountry, prodYear) = gettingComponentsResult.Value;
        if (prodCountry is null || prodYear is null)
        {
            return Compilation.Create(
                parent,
                type,
                identificator,
                discDirectoryInfo.FullName
                );
        }

        var creationDiscResult = Compilation.Create(
            parent,
            gettingComponentsResult.Value.type,
            gettingComponentsResult.Value.identificator,
            discDirectoryInfo.FullName,
            (int)gettingComponentsResult.Value.prodYear!,
            gettingComponentsResult.Value.prodCountry!);

        return creationDiscResult;
    }

    #endregion
}
