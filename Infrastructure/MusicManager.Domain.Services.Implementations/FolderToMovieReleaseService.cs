﻿using Microsoft.Extensions.Logging;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services.Storage;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;
using System.Collections.Concurrent;

namespace MusicManager.Domain.Services.Implementations;

public partial class FolderToMovieReleaseService : 
    BaseDiscDomainService,
    IFolderToMovieReleaseService
{
    #region --Fields--

    private readonly ConcurrentDictionary<string, MovieRelease> _cache = new();

	#endregion

	#region --Properties--



	#endregion

	#region --Constructors--

	public FolderToMovieReleaseService(IRoot userConfig, ILogger<FolderToMovieReleaseService> logger) : base(userConfig, logger)
	{
	}

	#endregion

	#region --Methods--

	public Task<Result<MovieRelease>> GetEntityAsync(string movieReleasePath)
    {
        var isAbleToMoveNextResult = IsAbleToParse<DirectoryInfo>(movieReleasePath);
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
            if (entityJsonResult.IsSuccess)
            {
                var entityResult = entityJsonResult.Value.ToEntity(movieReleasePath.GetRelational(_root));
                if (entityResult.IsFailure)
                {
                    Task.FromResult(Result.Failure<MovieRelease>(entityResult.Error));
                }

                // add to cache parsed from json entity
                _cache[movieReleasePath] = entityResult.Value;
                return Task.FromResult(Result.Success(entityResult.Value));
            }

            return Task.FromResult(Result.Failure<MovieRelease>(entityJsonResult.Error));
        }

        var movieReleaseCreationResult = directoryInfo.Name.Contains(DiscType.Bootleg.Value) ?
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
            discDirectoryInfo.FullName.GetRelational(_root));

        return diskCreationResult;
    }

    private Result<MovieRelease> CreateSimpleDisc(DirectoryInfo discDirectoryInfo)
    {
        var gettingComponentsResult = GetDiscComponentsFromFolderName(discDirectoryInfo.Name);

        if (gettingComponentsResult.IsFailure)
        {
            return Result.Failure<MovieRelease>(gettingComponentsResult.Error);
        }

        var (type, identifier, prodCountry, prodYear) = gettingComponentsResult.Value;
        var creationDiscResult = MovieRelease.Create(
            type,
            identifier,
            discDirectoryInfo.FullName.GetRelational(_root),
            prodYear,
            prodCountry);

        return creationDiscResult;
    }

    #endregion
}
