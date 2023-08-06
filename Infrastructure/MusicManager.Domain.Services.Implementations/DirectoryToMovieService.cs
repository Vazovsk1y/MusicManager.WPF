using MusicManager.Domain.Models;
using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Utils;

namespace MusicManager.Domain.Services.Implementations;

public class DirectoryToMovieService : IPathToMovieService
{
    private readonly char _separator = '-';

    public Task<Result<Movie>> GetEntityAsync(string moviePath, SongwriterId songwriterId)
    {
        var isAbleToMoveNext = IsAbleToMoveNext(moviePath);
        if (isAbleToMoveNext.IsFailure)
        {
            return Task.FromResult(Result.Failure<Movie>(isAbleToMoveNext.Error));
        }

        var directoryInfo = isAbleToMoveNext.Value;
        var (isInfoSuccessfullyExtracted, year, title) = GetMovieInfo(directoryInfo.Name);
        if (!isInfoSuccessfullyExtracted)
        {
            return Task.FromResult(Result.Failure<Movie>(DomainServicesErrors.PassedDirectoryNamedIncorrect(moviePath)));
        }

        var movieCreationResult = Movie.Create(
            songwriterId,
            title!,
            year,
            "Undefined",
            directoryInfo.FullName);

        if (movieCreationResult.IsFailure)
        {
            return Task.FromResult(Result.Failure<Movie>(movieCreationResult.Error));
        }

        return Task.FromResult(Result.Success(movieCreationResult.Value));
    }

    private (bool isSuccessfullyExtracted, int year, string? title) GetMovieInfo(string directoryName)
    {
        var info = directoryName
            .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
            .Select(i => i.TrimEnd().TrimStart())
            .ToList();

        _ = int.TryParse(info[0], out int result);
        return info.Count < 2 ? (false, result, null) : (true, result, info[1]);
    }

    private Result<DirectoryInfo> IsAbleToMoveNext(string moviePath)
    {
        if (!PathValidator.IsValid(moviePath))
        {
            return Result.Failure<DirectoryInfo>(DomainServicesErrors.PassedDirectoryPathIsInvalid(moviePath));
        }

        var directoryInfo = new DirectoryInfo(moviePath);
        if (!directoryInfo.Exists)
        {
            return Result.Failure<DirectoryInfo>(DomainServicesErrors.PassedDirectoryIsNotExists(moviePath));
        }

        return directoryInfo;
    }
}
