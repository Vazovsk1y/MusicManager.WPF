using MusicManager.Domain.Extensions;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Services.Implementations;

public class FolderToMovieService : 
    BaseDomainService,
    IFolderToMovieService
{
    #region --Fields--



    #endregion

    #region --Properties--



    #endregion

    #region --Constructors--

    public FolderToMovieService(IRoot userConfig) : base(userConfig)
    {
    }

    #endregion

    #region --Methods--

    public Task<Result<Movie>> GetEntityAsync(string moviePath, SongwriterId songwriterId)
    {
        var isAbleToMoveNextResult = IsAbleToMoveNext<DirectoryInfo>(moviePath);
        if (isAbleToMoveNextResult.IsFailure)
        {
            return Task.FromResult(Result.Failure<Movie>(isAbleToMoveNextResult.Error));
        }

        var directoryInfo = isAbleToMoveNextResult.Value;
        if (directoryInfo.EnumerateFiles().FirstOrDefault(e => e.Name == MovieEntityJson.FileName) is FileInfo fileInfo)
        {
            var entityJsonResult = GetEntityInfoFromJsonFile<MovieEntityJson, Movie>(fileInfo);
            return entityJsonResult.IsFailure ?
                Task.FromResult(Result.Failure<Movie>(entityJsonResult.Error))
                :
                Task.FromResult(entityJsonResult.Value.ToEntity(songwriterId, moviePath.GetRelational(_root)));
        }


        var (isInfoSuccessfullyExtracted, year, title) = GetMovieInfoFromDirectoryName(directoryInfo.Name);
        if (!isInfoSuccessfullyExtracted)
        {
            return Task.FromResult(Result.Failure<Movie>(DomainServicesErrors.PassedDirectoryNamedIncorrect(moviePath)));
        }

        var movieCreationResult = Movie.Create(
            songwriterId,
            title!,
            moviePath.GetRelational(_root),
            year);

        if (movieCreationResult.IsFailure)
        {
            return Task.FromResult(Result.Failure<Movie>(movieCreationResult.Error));
        }

        return Task.FromResult(Result.Success(movieCreationResult.Value));
    }

    private (bool isSuccessfullyExtracted, int year, string? title) GetMovieInfoFromDirectoryName(string directoryName)
    {
        var info = directoryName
            .Split(DomainServicesConstants.MovieDirectoryNameSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Select(i => i.TrimEnd().TrimStart())
            .ToList();

        if (info.Count < 2)
        {
            return (false, default, null);
        }

        _ = int.TryParse(info[0], out int result);
        return (true, result, info[1]);
    }

    #endregion
}
