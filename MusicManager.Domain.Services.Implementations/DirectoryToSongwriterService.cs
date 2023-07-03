using MusicManager.Domain.Helpers;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services.Implementations;

public class DirectoryToSongwriterService : IPathToSongwriterService
{
    private readonly char _separator = '.';

    public Task<Result<Songwriter>> GetEntityAsync(string songWriterPath)
    {
        var isAbleToMoveNext = IsAbleToMoveNext(songWriterPath);
        if (isAbleToMoveNext.IsFailure)
        {
            return Task.FromResult(Result.Failure<Songwriter>(isAbleToMoveNext.Error));
        }

        var directoryInfo = isAbleToMoveNext.Value;
        var (isInfoSuccessfullyExtracted, name, surname) = GetSongwriterInfo(directoryInfo.Name);
        if (!isInfoSuccessfullyExtracted)
        {
            return Task.FromResult(Result.Failure<Songwriter>(DomainServicesErrors.PassedDirectoryNamedIncorrect(songWriterPath)));
        }

        var songwriterCreationResult = Songwriter.Create(name!, surname!, directoryInfo.FullName);
        if (songwriterCreationResult.IsFailure)
        {
            return Task.FromResult(Result.Failure<Songwriter>(songwriterCreationResult.Error));
        }

        var songWriter = songwriterCreationResult.Value;
        return Task.FromResult(Result.Success(songWriter));
    }

    private (bool isInfoSuccessfullyExtracted, string? name, string? surname) GetSongwriterInfo(string directoryName)
    {
        var info = directoryName.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

        return info.Length < 2 ? (false, null, null) : (true, info[0], info[1]);
    }

    private Result<DirectoryInfo> IsAbleToMoveNext(string songWriterPath)
    {
        if (!PathValidator.IsValid(songWriterPath))
        {
            return Result.Failure<DirectoryInfo>(DomainServicesErrors.PassedDirectoryPathIsInvalid(songWriterPath));
        }

        var directoryInfo = new DirectoryInfo(songWriterPath);
        if (!directoryInfo.Exists)
        {
            return Result.Failure<DirectoryInfo>(DomainServicesErrors.PassedDirectoryIsNotExists(songWriterPath));
        }

        return directoryInfo;
    }
}
