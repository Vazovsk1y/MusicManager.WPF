using MusicManager.Domain.Enums;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using System.Collections.Concurrent;

namespace MusicManager.Domain.Services.Implementations;

public partial class DirectoryToMovieReleaseService : 
    BaseDiscDomainService,
    IPathToMovieReleaseService
{
    #region --Fields--

    private readonly ConcurrentDictionary<string, MovieRelease> _cache = new();

    #endregion

    #region --Properties--



    #endregion

    #region --Constructors--



    #endregion

    #region --Methods--

    public Task<Result<MovieRelease>> GetEntityAsync(string movieReleasePath)
    {
        var isAbleToMoveNextResult = IsAbleToMoveNext<DirectoryInfo>(movieReleasePath);
        if (isAbleToMoveNextResult.IsFailure)
        {
            return Task.FromResult(Result.Failure<MovieRelease>(isAbleToMoveNextResult.Error));
        }

        if (_cache.TryGetValue(movieReleasePath, out var value))
        {
            return Task.FromResult(Result.Success(value));
        }

        var directoryInfo = isAbleToMoveNextResult.Value;
        if (directoryInfo.EnumerateFiles().FirstOrDefault(e => e.Name == MovieReleaseEntityJson.FileName) is FileInfo fileInfo)
        {
            var entityJsonResult = GetEntityInfoFromJsonFile<MovieReleaseEntityJson, MovieRelease>(fileInfo);
            return entityJsonResult.IsFailure ?
                Task.FromResult(Result.Failure<MovieRelease>(entityJsonResult.Error))
                :
                Task.FromResult(entityJsonResult.Value.ToEntity(movieReleasePath));
        }

        var movieReleaseCreationResult = directoryInfo.Name.Contains(DiscType.Bootleg.ToString()) ?
            CreateBootLeg(directoryInfo) : CreateSimpleDisc(directoryInfo);

        if (movieReleaseCreationResult.IsSuccess)
        {
            _cache[movieReleasePath] = movieReleaseCreationResult.Value;
            return Task.FromResult(Result.Success(movieReleaseCreationResult.Value));
        }

        return Task.FromResult(Result.Failure<MovieRelease>(movieReleaseCreationResult.Error));
    }

    private Result<MovieRelease> CreateBootLeg(DirectoryInfo discDirectoryInfo)
    {
        var diskCreationResult = MovieRelease.Create(
            DiscType.Bootleg,
            discDirectoryInfo.Name,
            discDirectoryInfo.FullName);

        return diskCreationResult;
    }

    private Result<MovieRelease> CreateSimpleDisc(DirectoryInfo discDirectoryInfo)
    {
        var gettingComponentsResult = GetDiscComponentsFromDirectoryName(discDirectoryInfo.Name);

        if (gettingComponentsResult.IsFailure)
        {
            return Result.Failure<MovieRelease>(gettingComponentsResult.Error);
        }

        var creationDiscResult = MovieRelease.Create(
            gettingComponentsResult.Value.type,
            gettingComponentsResult.Value.identificator,
            discDirectoryInfo.FullName,
            gettingComponentsResult.Value.prodYear,
            gettingComponentsResult.Value.prodCountry);

        return creationDiscResult;
    }

    #endregion
}
