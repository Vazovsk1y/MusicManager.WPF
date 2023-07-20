using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Base;
using MusicManager.Services.Contracts.Factories;

namespace MusicManager.Services.Implementations.Contracts.Factories;

public class SongwriterFolderFactory : ISongwriterFolderFactory
{
    private const string MOVIES_FOLDER_NAME = "movies";
    private const string COMPILATIONS_FOLDER_NAME = "compilations";

    private readonly IMovieFolderFactory _movieFolderFactory;
    private readonly IDiscFolderFactory _compilationFolderFactory;

    public SongwriterFolderFactory(
        IMovieFolderFactory movieFolderFactory, 
        IDiscFolderFactory discFolderFactory)
    {
        _movieFolderFactory = movieFolderFactory;
        _compilationFolderFactory = discFolderFactory;
    }

    public Result<SongwriterFolder> Create(DirectoryInfo songwriterDirectory)
    {
        if (!songwriterDirectory.Exists) 
        {
            return Result.Failure<SongwriterFolder>(DomainServicesErrors.PassedDirectoryIsNotExists(songwriterDirectory.FullName));
        }

        List<MovieFolder> moviesFolders = new();
        List<DiscFolder> compilationsFolders = new();
        var subDirectories = songwriterDirectory.EnumerateDirectories();
        var moviesDirectory = subDirectories.FirstOrDefault(d => d.Name == MOVIES_FOLDER_NAME);
        var compilationsDirectory = subDirectories.FirstOrDefault(d => d.Name == COMPILATIONS_FOLDER_NAME);
        
        if (moviesDirectory is not null)
        {
            var moviesDirectories = moviesDirectory.EnumerateDirectories();
            foreach (var movieFolder in moviesDirectories)
            {
                var result = _movieFolderFactory.Create(movieFolder);
                if (result.IsFailure)
                {
                    return Result.Failure<SongwriterFolder>(result.Error);
                }

                moviesFolders.Add(result.Value);
            }
        }

        if (compilationsDirectory is not null)
        {
            var compilationsDirectories = compilationsDirectory.EnumerateDirectories();
            foreach (var compilationFolder in compilationsDirectories)
            {
                var result = _compilationFolderFactory.Create(compilationFolder);
                if (result.IsFailure)
                {
                    return Result.Failure<SongwriterFolder>(result.Error);
                }

                compilationsFolders.Add(result.Value);
            }
        }

        return new SongwriterFolder(
            songwriterDirectory.FullName, 
            moviesFolders, 
            compilationsFolders);
    }
}
