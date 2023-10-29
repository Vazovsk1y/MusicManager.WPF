using MusicManager.Domain.Services;
using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Base;
using MusicManager.Services.Contracts.Factories;
using System.IO;

namespace MusicManager.Services.Implementations.Contracts.Factories;

public class SongwriterFolderFactory : ISongwriterFolderFactory
{
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

        var subDirectories = songwriterDirectory.EnumerateDirectories();
        var moviesDirectory = subDirectories.FirstOrDefault(d => string.Equals(d.Name , DomainServicesConstants.MOVIES_FOLDER_NAME, StringComparison.OrdinalIgnoreCase));
        var compilationsDirectory = subDirectories.FirstOrDefault(d => string.Equals(d.Name, DomainServicesConstants.COMPILATIONS_FOLDER_NAME, StringComparison.OrdinalIgnoreCase));

        var moviesResult = GetMoviesFolders(moviesDirectory);
        if (moviesResult.IsFailure)
        {
            return Result.Failure<SongwriterFolder>(moviesResult.Error);
		}

        var compilationsResult = GetCompilationsFolders(compilationsDirectory);
        if (compilationsResult.IsFailure)
        {
            return Result.Failure<SongwriterFolder>(compilationsResult.Error);
        }

        return new SongwriterFolder(
            songwriterDirectory.FullName, 
            moviesResult.Value, 
            compilationsResult.Value);
    }

    private Result<IReadOnlyCollection<MovieFolder>> GetMoviesFolders(DirectoryInfo? moviesDirectory)
    {
        if (moviesDirectory is null)
        {
            return Result.Failure<IReadOnlyCollection<MovieFolder>>(new($"Required \"{DomainServicesConstants.MOVIES_FOLDER_NAME}\" folder is not exists."));
        }

		List<MovieFolder> moviesFolders = new();
		var moviesDirectories = moviesDirectory.EnumerateDirectories();
		foreach (var movieFolder in moviesDirectories)
		{
			var result = _movieFolderFactory.Create(movieFolder);
			if (result.IsFailure)
			{
				return Result.Failure<IReadOnlyCollection<MovieFolder>>(result.Error);
			}

			moviesFolders.Add(result.Value);
		}

        return moviesFolders;
	}

	private Result<IReadOnlyCollection<DiscFolder>> GetCompilationsFolders(DirectoryInfo? compilationsDirectory)
	{
		if (compilationsDirectory is null)
		{
			return Result.Failure<IReadOnlyCollection<DiscFolder>>(new($"Required \"{DomainServicesConstants.COMPILATIONS_FOLDER_NAME}\" folder is not exists."));
		}

		List<DiscFolder> compilationsFolders = new();
		var compilationsDirectories = compilationsDirectory.EnumerateDirectories();
		foreach (var compilationFolder in compilationsDirectories)
		{
			var result = _compilationFolderFactory.Create(compilationFolder);
			if (result.IsFailure)
			{
				return Result.Failure<IReadOnlyCollection<DiscFolder>>(result.Error);
			}

			compilationsFolders.Add(result.Value);
		}

		return compilationsFolders;
	}
}
