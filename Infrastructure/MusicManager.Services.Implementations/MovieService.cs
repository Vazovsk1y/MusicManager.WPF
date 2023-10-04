using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services;
using MusicManager.Domain.Shared;
using MusicManager.Repositories.Data;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Services.Extensions;

namespace MusicManager.Services.Implementations;

public class MovieService : IMovieService
{
    private readonly IFolderToMovieService _pathToMovieService;
    private readonly IMovieReleaseService _movieReleaseService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMovieToFolderService _movieToFolderService;
    private readonly IMovieReleaseToFolderService _movieReleaseToFolderService;

    public MovieService(
        IFolderToMovieService pathToMovieService,
        IMovieReleaseService movieReleaseService,
        IApplicationDbContext dbContext,
        IMovieToFolderService movieToFolderService,
        IMovieReleaseToFolderService movieReleaseToFolderService)
    {
        _pathToMovieService = pathToMovieService;
        _movieReleaseService = movieReleaseService;
        _dbContext = dbContext;
        _movieToFolderService = movieToFolderService;
        _movieReleaseToFolderService = movieReleaseToFolderService;
    }

    public async Task<Result> AddReleaseAsync(MovieReleaseMovieDTO dto, bool createAssociatedLink = true, CancellationToken cancellationToken = default)
    {
        var movie = await _dbContext
            .Movies
            .Include(e => e.ReleasesLinks)
            .ThenInclude(e => e.MovieRelease)
            .ThenInclude(e => e.MoviesLinks)
            .SingleOrDefaultAsync(e => e.Id == dto.MovieId, cancellationToken)
            .ConfigureAwait(false);

        if (movie is null)
        {
            return Result.Failure(ServicesErrors.MovieWithPassedIdIsNotExists());
        }

        var movieRelease = await _dbContext
            .MoviesReleases
            .Include(e => e.MoviesLinks)
            .SingleOrDefaultAsync(e => e.Id == dto.DiscId, cancellationToken);

        if (movieRelease is null)
        {
            return Result.Failure(ServicesErrors.MovieReleaseWithPassedIdIsNotExists());
        }

        if (createAssociatedLink)
        {
			var settingLinkResult = await _movieReleaseToFolderService.CreateFolderLinkAsync(movieRelease, movie, cancellationToken);
			if (settingLinkResult.IsFailure)
			{
				return settingLinkResult;
			}

			var addingResult = movie.AddRelease(movieRelease, settingLinkResult.Value);
			if (addingResult.IsFailure)
			{
				return addingResult;
			}
		}
        else
        {
			var addingResult = movie.AddRelease(movieRelease);
			if (addingResult.IsFailure)
			{
				return addingResult;
			}
		}
       
		await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(MovieId movieId, CancellationToken cancellationToken = default)
    {
        var movie = await _dbContext.Movies
            .Include(e => e.ReleasesLinks)
            .SingleOrDefaultAsync(e => e.Id == movieId, cancellationToken);

        if (movie is null)
        {
            return Result.Failure(ServicesErrors.MovieWithPassedIdIsNotExists());
        }

        var moviesReleasesToRemove = _dbContext.MoviesReleases
            .Include(e => e.MoviesLinks)
            .ThenInclude(e => e.MovieRelease)
            .ThenInclude(e => e.Songs)
            .Where(e => e.MoviesLinks.Any(e => e.MovieId == movieId) && e.MoviesLinks.Count == 1);

        _dbContext.Movies.Remove(movie);
        _dbContext.MoviesReleases.RemoveRange(moviesReleasesToRemove);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<IReadOnlyCollection<MovieDTO>>> GetAllAsync(SongwriterId parentId, CancellationToken cancellation = default)
    {
        var result = await _dbContext.Movies
            .AsNoTracking()
            .Where(e => e.SongwriterId == parentId)
            .Select(e => e.ToDTO())
            .ToListAsync(cancellation);

        return result;
    }

    public async Task<Result<IReadOnlyCollection<MovieLookupDTO>>> GetLookupsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Movies
            .AsNoTracking()
            .Select(e => e.ToLookupDTO())
            .ToListAsync(cancellationToken); 
    }

    public async Task<Result<MovieId>> SaveAsync(MovieAddDTO movieAddDTO, bool createAssociatedFolder = true, CancellationToken cancellationToken = default)
    {
        var songwriter = await _dbContext
            .Songwriters
            .Include(e => e.Movies)
            .SingleOrDefaultAsync(e => e.Id == movieAddDTO.SongwriterId, cancellationToken);

        if (songwriter is null)
        {
            return Result.Failure<MovieId>(ServicesErrors.SongwriterWithPassedIdIsNotExists());
        }

        var movieCreationResult = Movie.Create(
            movieAddDTO.SongwriterId,
            movieAddDTO.Title,
            movieAddDTO.ProductionYear,
            movieAddDTO.ProductionCountry
            );

        if (movieCreationResult.IsFailure)
        {
            return Result.Failure<MovieId>(movieCreationResult.Error);
        }

        var movie = movieCreationResult.Value;
        if (createAssociatedFolder)
        {
            var createdAssociatedFolderAndFileResult = await _movieToFolderService.CreateAssociatedFolderAndFileAsync(movie, songwriter, cancellationToken);
            if (createdAssociatedFolderAndFileResult.IsFailure)
            {
                return Result.Failure<MovieId>(createdAssociatedFolderAndFileResult.Error);
            }

            movie.SetAssociatedFolder(createdAssociatedFolderAndFileResult.Value);
        }
       
        songwriter.AddMovie(movie);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(movie.Id);
    }

    public async Task<Result<MovieDTO>> SaveFromFolderAsync(MovieFolder movieFolder, SongwriterId songwriterId, CancellationToken cancellationToken = default)
    {
        var movieResult = await _pathToMovieService
            .GetEntityAsync(movieFolder.Path, songwriterId)
            .ConfigureAwait(false);

        if (movieResult.IsFailure)
        {
            return Result.Failure<MovieDTO>(movieResult.Error);
        }

        var movie = movieResult.Value;
        var songwriter = await _dbContext
            .Songwriters
            .Include(e => e.Movies)
            .SingleOrDefaultAsync(e => e.Id == movie.SongwriterId, cancellationToken);

        if (songwriter is null)
        {
            return Result.Failure<MovieDTO>(ServicesErrors.SongwriterWithPassedIdIsNotExists());
        }

        var addingResult = songwriter.AddMovie(movie);
        if (addingResult.IsFailure)
        {
            return Result.Failure<MovieDTO>(addingResult.Error);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        foreach (var movieReleaseFolder in movieFolder.MoviesReleasesFolders)
        {
            var result = await _movieReleaseService.SaveFromFolderAsync(movieReleaseFolder, movie.Id, cancellationToken);
            if (result.IsFailure)
            {
                return Result.Failure<MovieDTO>(result.Error);
            }
        }

        return movie.ToDTO();
    }

    public async Task<Result> UpdateAsync(MovieUpdateDTO movieUpdateDTO, CancellationToken cancellationToken = default)
    {
        var movie = await _dbContext
            .Movies
            .Include(e => e.Director)
            .ThenInclude(e => e == null ? null : e.Movies)
            .SingleOrDefaultAsync(e => e.Id == movieUpdateDTO.Id, cancellationToken);

        if (movie is null)
        {
            return Result.Failure(ServicesErrors.MovieWithPassedIdIsNotExists());
        }

        var updateActions = new List<Result>()
        {
            movie.SetTitle(movieUpdateDTO.Title),
            movie.SetProductionInfo(movieUpdateDTO.ProductionCountry, movieUpdateDTO.ProductionYear),
        };

        if (movieUpdateDTO.DirectorId != null)
        {
            var director = await _dbContext
                .Directors
                .Include(e => e.Movies)
                .SingleOrDefaultAsync(e => e.Id == movieUpdateDTO.DirectorId, cancellationToken);

            if (director is null)
            {
                return Result.Failure(new Error("Director with passed id isnt exists in database."));
            }

            updateActions.Add(movie.SetDirector(director));
        }

        if (updateActions.Any(e => e.IsFailure))
        {
            return Result.Failure(new(string.Join("\n", updateActions.Where(e => e.IsFailure).Select(e => e.Error.Message))));
        }

        if (movie.AssociatedFolderInfo is not null)
        {
			var folderUpdatingResult = await _movieToFolderService.UpdateAsync(movie);
			if (folderUpdatingResult.IsFailure)
			{
                return folderUpdatingResult;
			}

			movie.SetAssociatedFolder(folderUpdatingResult.Value);
		}

		await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}



