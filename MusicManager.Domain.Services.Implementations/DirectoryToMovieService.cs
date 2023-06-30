using MusicManager.Domain.Helpers;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services.Implementations;

public class DirectoryToMovieService : IPathToMovieService
{
    private readonly char _separator = '-';

    public Task<Result<Movie>> GetEntityAsync(string moviePath, SongwriterId parent)
    {
        if (!PathValidator.IsValid(moviePath))
        {
            return Task.FromResult(Result.Failure<Movie>(DomainServicesErrors.PassedDirectoryPathIsInvalid(moviePath)));
        }

        var directoryInfo = new DirectoryInfo(moviePath);
        if (!directoryInfo.Exists)
        {
            return Task.FromResult(Result.Failure<Movie>(DomainServicesErrors.PassedDirectoryIsNotExists(moviePath)));
        }

        var (isInfoSuccessfullyExtracted, year, title) = GetMovieInfo(directoryInfo.Name);
        if (!isInfoSuccessfullyExtracted)
        {
            return Task.FromResult(Result.Failure<Movie>(DomainServicesErrors.PassedDirectoryNamedIncorrect(moviePath)));
        }

        var movieCreationResult = Movie.Create(
            parent,
            title!,
            year!,
            "Undefined",
            directoryInfo.FullName);

        if (movieCreationResult.IsFailure)
        {
            return Task.FromResult(Result.Failure<Movie>(movieCreationResult.Error));
        }

        return Task.FromResult(Result.Success(movieCreationResult.Value));
    }

    private (bool isSuccessfullyExtracted, string? year, string? title) GetMovieInfo(string directoryName)
    {
        var info = directoryName
            .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
            .Select(i => i.TrimEnd().TrimStart())
            .ToList();

        return info.Count < 2 ? (false, null, null) : (true, info[0], info[1]);
    }
}
