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
using MusicManager.Utils;

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
            .Include(e => e.MoviesLinks)
            .SingleOrDefaultAsync(e => e.Id == discId, cancellationToken);

        if (movieRelease is null)
        {
            return Result.Failure(ServicesErrors.MovieReleaseWithPassedIdIsNotExists());
        }

        _dbContext.MovieReleases.Remove(movieRelease);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
	}

	public async Task<Result<IEnumerable<MovieReleaseLinkDTO>>> GetAllAsync(MovieId movieId, CancellationToken cancellationToken = default)
    {
        var result = new List<MovieReleaseLinkDTO>();
        var movie = await _dbContext
            .Movies
            .Include(e => e.ReleasesLinks)
            .ThenInclude(e => e.MovieRelease)
            .SingleOrDefaultAsync(e => e.Id == movieId, cancellationToken);

        if (movie is null)
        {
            return Result.Failure<IEnumerable<MovieReleaseLinkDTO>>(ServicesErrors.MovieWithPassedIdIsNotExists());
        }

        var moviesReleases = movie.ReleasesLinks;
        foreach (var movieReleaseLink in moviesReleases)
        {
            var songsResult = await _songService.GetAllAsync(movieReleaseLink.MovieRelease.Id, cancellationToken);
            if (songsResult.IsFailure)
            {
                return Result.Failure<IEnumerable<MovieReleaseLinkDTO>>(songsResult.Error);
            }

            var movieReleaseDTO = movieReleaseLink.MovieRelease.ToDTO() with
            {
                SongDTOs = songsResult.Value
            };

            result.Add(new MovieReleaseLinkDTO(movieReleaseDTO, movieReleaseLink.ReleaseLink is null));
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
            .Include(e => e.ReleasesLinks)
            .ThenInclude(e => e.MovieRelease)
            .ThenInclude(e => e.MoviesLinks)
            .Where(e => movieReleaseAddDTO.MoviesLinks.Select(e => e.MovieId).Contains(e.Id))
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
        if (createAssociatedFolder)
        {
            var moviesWhereWillBeStoresActualFolder = movies.Where(e => movieReleaseAddDTO.MoviesLinks.Any(item => item.AddAsFolder && e.Id == item.MovieId));
            if (!moviesWhereWillBeStoresActualFolder.Any())
            {
                return Result.Failure<DiscId>(new Error("Movie where will be store the original movie release folder is not found."));
            }

            InvalidOperationExceptionHelper.ThrowIfTrue(moviesWhereWillBeStoresActualFolder.Count() > 1, "Detected more than one movie where actual folder must be created.");

            var firstMovie = moviesWhereWillBeStoresActualFolder.First();
            var createMovieReleaseFolderResult = await _movieReleaseToFolderService.CreateAssociatedFolderAndFileAsync(movieRelease, firstMovie);
            if (createMovieReleaseFolderResult.IsFailure)
            {
                return Result.Failure<DiscId>(createMovieReleaseFolderResult.Error);
            }

            movies.Remove(firstMovie);
            var settingLinksResult = await SetLinks(movies, createMovieReleaseFolderResult.Value, movieRelease);  // foreach other movies we create a folder links instead real folder.
            if (settingLinksResult.IsFailure)
            {
                return Result.Failure<DiscId>(settingLinksResult.Error);
            }

            firstMovie.AddRelease(movieRelease);
            movieRelease.SetDirectoryInfo(createMovieReleaseFolderResult.Value);
        }
        else
        {
            var addingToMoviesResult = AddToMovies(movies, movieRelease);
            if (addingToMoviesResult.IsFailure)
            {
                return Result.Failure<DiscId>(addingToMoviesResult.Error);
            }
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
            .Include(e => e.ReleasesLinks)
            .ThenInclude(e => e.MovieRelease)
            .ThenInclude(e => e.MoviesLinks)
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

        var addingResult = movie.AddRelease(movieRelease, realeseLinkPath: movieReleaseFolder.LinkPath?.GetRelational(_root));
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
            .Include(e => e.MoviesLinks)
            .ThenInclude(e => e.Movie)
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

    private async Task<Result> SetLinks(IEnumerable<Movie> movies, string movieReleaseRelationalPath, MovieRelease movieRelease)
    {
        foreach (var movie in movies) 
        {
            var settingLinkResult = await _movieReleaseToFolderService.CreateFolderLinkAsync(movie, movieReleaseRelationalPath);
            if (settingLinkResult.IsFailure)
            {
                return Result.Failure(settingLinkResult.Error);
            }

            movie.AddRelease(movieRelease, realeseLinkPath: settingLinkResult.Value);
        }

        return Result.Success();
    }
}
