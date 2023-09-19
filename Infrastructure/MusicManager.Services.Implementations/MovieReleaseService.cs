using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Common;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services;
using MusicManager.Domain.Shared;
using MusicManager.Repositories.Data;
using MusicManager.Services.Contracts.Base;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Services.Extensions;

namespace MusicManager.Services.Implementations;

public class MovieReleaseService : IMovieReleaseService
{
    private readonly IFolderToMovieReleaseService _pathToMovieReleaseService;
    private readonly ISongService _songService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMovieReleaseToFolderService _movieReleaseToFolderService;
    private readonly IRoot _root;

    public MovieReleaseService(
        IFolderToMovieReleaseService pathToMovieReleaseService,
        ISongService songService,
        IApplicationDbContext dbContext,
        IMovieReleaseToFolderService movieReleaseToFolderService,
        IRoot root)
    {
        _pathToMovieReleaseService = pathToMovieReleaseService;
        _songService = songService;
        _dbContext = dbContext;
        _movieReleaseToFolderService = movieReleaseToFolderService;
        _root = root;
    }

	public async Task<Result> DeleteAsync(DiscId discId, CancellationToken cancellationToken = default)
	{
        var movieRelease = await _dbContext.MovieReleases
            .SingleOrDefaultAsync(e => e.Id == discId, cancellationToken);

        if (movieRelease is null)
        {
            return Result.Failure(ServicesErrors.MovieReleaseWithPassedIdIsNotExists());
        }

        _dbContext.MovieReleases.Remove(movieRelease);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
	}

	public async Task<Result<IEnumerable<MovieReleaseDTO>>> GetAllAsync(MovieId movieId, CancellationToken cancellationToken = default)
    {
        var result = new List<MovieReleaseDTO>();
        var movie = await _dbContext
            .Movies
            .Include(e => e.Releases)
            .SingleOrDefaultAsync(e => e.Id == movieId, cancellationToken);

        if (movie is null)
        {
            return Result.Failure<IEnumerable<MovieReleaseDTO>>(ServicesErrors.MovieWithPassedIdIsNotExists());
        }

        var moviesReleases = movie.Releases;
        foreach (var movieRelease in moviesReleases)
        {
            var songsResult = await _songService.GetAllAsync(movieRelease.Id, cancellationToken);
            if (songsResult.IsFailure)
            {
                return Result.Failure<IEnumerable<MovieReleaseDTO>>(songsResult.Error);
            }

            result.Add(movieRelease.ToDTO() with
            {
                SongDTOs = songsResult.Value
            });
        }

        return result;
    }

    public async Task<Result<DiscId>> SaveAsync(MovieReleaseAddDTO movieReleaseAddDTO, bool createAssociatedFolder = true, CancellationToken cancellationToken = default)
    {
        bool hasDuplicates = movieReleaseAddDTO.MoviesLinks.GroupBy(x => x).Any(g => g.Count() > 1);
        if (hasDuplicates)
        {
            return Result.Failure<DiscId>(new("Can't add the same movie release to the one movie twice.\nMovies links contains duplicates."));
        }

        var movies = await _dbContext
            .Movies
            .Include(e => e.Releases)
            .ThenInclude(e => e.Movies)
            .Where(e => movieReleaseAddDTO.MoviesLinks.Contains(e.Id))
            .ToListAsync(cancellationToken);

        if (movies.Count is 0)
        {
            return Result.Failure<DiscId>(new Error("No movie were selected."));
        }

        var creationResult = MovieRelease.Create(
            movieReleaseAddDTO.DiscType,
            movieReleaseAddDTO.Identifier,
            movieReleaseAddDTO.ProductionYear,
            movieReleaseAddDTO.ProductionCountry
            );

        if (creationResult.IsFailure)
        {
            return Result.Failure<DiscId>(creationResult.Error);
        }

        var movieRelease = creationResult.Value;
        var addingToMoviesResult = AddToMovies(movies, movieRelease);
        if (addingToMoviesResult.IsFailure)
        {
            return Result.Failure<DiscId>(addingToMoviesResult.Error);
        }

        if (createAssociatedFolder)
        {
            var firstMovie = movies.First();
            var createMovieReleaseFolderResult = await _movieReleaseToFolderService.CreateAssociatedFolderAndFileAsync(movieRelease, firstMovie);
            if (createMovieReleaseFolderResult.IsFailure)
            {
                return Result.Failure<DiscId>(createMovieReleaseFolderResult.Error);
            }

            var settingLinksResult = await SetLinks(movies.Skip(1), createMovieReleaseFolderResult.Value);     // foreach other movies we create a folder links instead real folder.
            if (settingLinksResult.IsFailure)
            {
                return Result.Failure<DiscId>(settingLinksResult.Error);
            }

            movieRelease.SetDirectoryInfo(createMovieReleaseFolderResult.Value);
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(movieRelease.Id);
    }

    public async Task<Result<MovieReleaseDTO>> SaveFromFolderAsync(DiscFolder movieReleaseFolder, MovieId movieId, CancellationToken cancellationToken = default)
    {
        var movieReleaseResult = await _pathToMovieReleaseService
            .GetEntityAsync(movieReleaseFolder.Path)
            .ConfigureAwait(false);

        if (movieReleaseResult.IsFailure)
        {
            return Result.Failure<MovieReleaseDTO>(movieReleaseResult.Error);
        }

        var movieRelease = movieReleaseResult.Value;
        var movie = await _dbContext
            .Movies
            .Include(e => e.Releases)
            .ThenInclude(e => e.Movies)
            .SingleOrDefaultAsync(e => e.Id == movieId, cancellationToken);

        if (movie is null)
        {
            return Result.Failure<MovieReleaseDTO>(ServicesErrors.MovieWithPassedIdIsNotExists());
        }

        foreach (var coverPath in movieReleaseFolder.CoversPaths)
        {
            if (_root.IsStoresIn(coverPath))
            {
                movieRelease.AddCover(coverPath.GetRelational(_root));
            }
        }

        var addingResult = movie.AddRelease(movieRelease);
        if (addingResult.IsFailure)
        {
            return Result.Failure<MovieReleaseDTO>(addingResult.Error);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var songsDtos = new List<SongDTO>();    
        foreach (var songFile in movieReleaseFolder.Songs)
        {
            var result = await _songService.SaveFromFileAsync(songFile, movieRelease.Id, true, cancellationToken);

            if (result.IsFailure)
            {
                return Result.Failure<MovieReleaseDTO>(result.Error);
            }

            songsDtos.AddRange(result.Value);
        }

        return movieRelease.ToDTO() with
        {
            SongDTOs = songsDtos,
        };
    }

    public async Task<Result> UpdateAsync(MovieReleaseUpdateDTO movieReleaseUpdateDTO, CancellationToken cancellationToken = default)
    {
        var movieRelease = await _dbContext
            .MovieReleases
            .Include(e => e.Movies)
            .SingleOrDefaultAsync(e => e.Id == movieReleaseUpdateDTO.Id, cancellationToken);

        if (movieRelease is null)
        {
            return Result.Failure(ServicesErrors.MovieReleaseWithPassedIdIsNotExists());
        }

        var updateActions = new List<Result>()
        {
            movieRelease.SetIdentifier(movieReleaseUpdateDTO.Identifier),
            movieRelease.SetDiscType(movieReleaseUpdateDTO.DiscType),
            movieRelease.SetProductionInfo(movieReleaseUpdateDTO.ProductionCountry, movieReleaseUpdateDTO.ProductionYear)
        };

        if (updateActions.Any(e => e.IsFailure))
        {
            return Result.Failure(new(string.Join("\n", updateActions.Where(e => e.IsFailure).Select(e => e.Error.Message))));
        }

		if (movieRelease.EntityDirectoryInfo is not null)
		{
			var folderUpdatingResult = await _movieReleaseToFolderService.UpdateIfExistsAsync(movieRelease);
			if (folderUpdatingResult.IsFailure)
			{
                return folderUpdatingResult;
			}

			movieRelease.SetDirectoryInfo(folderUpdatingResult.Value);
		}

		await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private Result AddToMovies(IEnumerable<Movie> movies, MovieRelease movieRelease)
    {
        foreach (var movie in movies)
        {
            var result = movie.AddRelease(movieRelease);
            if (result.IsFailure)
            {
                return result;
            }
        }

        return Result.Success();
    }

    private async Task<Result> SetLinks(IEnumerable<Movie> movies, string movieReleaseRelationalPath)
    {
        foreach (var movie in movies) 
        {
            var settingLinkResult = await _movieReleaseToFolderService.CreateFolderLinkAsync(movie, movieReleaseRelationalPath);
            if (settingLinkResult.IsFailure)
            {
                return Result.Failure(settingLinkResult.Error);
            }
        }

        return Result.Success();
    }
}
