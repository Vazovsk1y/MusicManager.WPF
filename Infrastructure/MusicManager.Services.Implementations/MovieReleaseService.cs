﻿using Microsoft.EntityFrameworkCore;
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
    private readonly IMovieReleaseToFolderService _movieReleaseToFolderService;
    private readonly IApplicationDbContext _dbContext;
    private readonly ISongService _songService;
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

	public async Task<Result<MovieReleaseDTO>> GetAsync(DiscId discId, CancellationToken cancellationToken = default)
    {
        var movieRelease = await _dbContext
            .MoviesReleases
            .SingleOrDefaultAsync(e => e.Id == discId, cancellationToken);

        if (movieRelease is null)
        {
            return Result.Failure<MovieReleaseDTO>(ServicesErrors.MovieReleaseWithPassedIdIsNotExists());
        }

        return movieRelease.ToDTO();
    }

	public async Task<Result> DeleteAsync(DiscId discId, CancellationToken cancellationToken = default)
	{
        var movieRelease = await _dbContext.MoviesReleases
            .Include(e => e.MoviesLinks)
            .Include(e => e.Songs)
            .Include(e => e.Covers)
            .SingleOrDefaultAsync(e => e.Id == discId, cancellationToken);

        if (movieRelease is null)
        {
            return Result.Failure(ServicesErrors.MovieReleaseWithPassedIdIsNotExists());
        }

        _dbContext.MoviesReleases.Remove(movieRelease);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
	}

	public async Task<Result<IReadOnlyCollection<MovieReleaseLinkDTO>>> GetLinksAsync(MovieId parentId, CancellationToken cancellationToken = default)
    {
        var movie = await _dbContext
            .Movies
            .AsNoTracking()
            .Include(e => e.ReleasesLinks)
            .ThenInclude(e => e.MovieRelease)
            .SingleOrDefaultAsync(e => e.Id == parentId, cancellationToken);

        if (movie is null)
        {
            return Result.Failure<IReadOnlyCollection<MovieReleaseLinkDTO>>(ServicesErrors.MovieWithPassedIdIsNotExists());
        }

        return movie.ReleasesLinks.Select(e => e.ToDTO()).ToList();
    }

	public async Task<Result<IReadOnlyCollection<MovieReleaseLookupDTO>>> GetLookupsAsync(CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.
            MoviesReleases
            .AsNoTracking()
            .Select(e => new MovieReleaseLookupDTO(e.Id, e.Identifier, e.Type))
            .ToListAsync(cancellationToken);

        return result;
    }

	public async Task<Result<DiscId>> SaveAsync(MovieReleaseAddDTO movieReleaseAddDTO, bool createAssociatedFolder = true, CancellationToken cancellationToken = default)
    {
        var associatedMoviesLinks = movieReleaseAddDTO.AssociatedMoviesLinks.ToList();
		bool hasDuplicates = associatedMoviesLinks.GroupBy(x => x).Any(g => g.Count() > 1);

        if (hasDuplicates)
        {
            return Result.Failure<DiscId>(new("Can't add the same movie release to the one movie twice.\nMovies links contains duplicates."));
        }

        var movies = await _dbContext
            .Movies
            .Include(e => e.ReleasesLinks)
            .ThenInclude(e => e.MovieRelease)
            .ThenInclude(e => e.MoviesLinks)
            .Where(e => associatedMoviesLinks.Select(e => e.MovieId).Contains(e.Id))
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
            var result = await CreateFileSystemRelations(movies, movieRelease, associatedMoviesLinks);
        }
        else
        {
			foreach (var movie in movies)
			{
				var result = movie.AddRelease(movieRelease);
				if (result.IsFailure)
				{
					return Result.Failure<DiscId>(result.Error);
				}
			}
		}

		await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(movieRelease.Id);
    }

    public async Task<Result<MovieReleaseDTO>> SaveFromFolderAsync(DiscFolder movieReleaseFolder, MovieId parentId, CancellationToken cancellationToken = default)
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
            .SingleOrDefaultAsync(e => e.Id == parentId, cancellationToken);

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

        var addingResult = movie.AddRelease(movieRelease, releaseLinkPath: movieReleaseFolder.LinkPath?.GetRelational(_root));
        if (addingResult.IsFailure)
        {
            return Result.Failure<MovieReleaseDTO>(addingResult.Error);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        foreach (var songFile in movieReleaseFolder.SongsFiles)
        {
            var result = await _songService.SaveFromFileAsync(songFile, movieRelease.Id, true, cancellationToken);
            if (result.IsFailure)
            {
                return Result.Failure<MovieReleaseDTO>(result.Error);
            }
        }

        return movieRelease.ToDTO();
    }

    public async Task<Result> UpdateAsync(MovieReleaseUpdateDTO movieReleaseUpdateDTO, CancellationToken cancellationToken = default)
    {
        var movieRelease = await _dbContext
            .MoviesReleases
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

		if (movieRelease.AssociatedFolderInfo is not null)
		{
			var fileSystemRelationsUpdateResult = await _movieReleaseToFolderService.UpdateAsync(movieRelease);
			if (fileSystemRelationsUpdateResult.IsFailure)
			{
                return fileSystemRelationsUpdateResult;
			}

			movieRelease.SetAssociatedFolder(fileSystemRelationsUpdateResult.Value);
		}

		await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result> CreateFileSystemRelations(IList<Movie> movies, MovieRelease movieRelease, IList<MovieLinkDTO> associatedMoviesLinks)
    {
		var moviesWhereWillBeStoreActualFolder = movies.Where(e => associatedMoviesLinks.Any(item => item.AddReleaseAsFolder && e.Id == item.MovieId));

		InvalidOperationExceptionHelper.ThrowIfTrue(!moviesWhereWillBeStoreActualFolder.Any(), "Movie where will be store the original movie release folder is not found.");
		InvalidOperationExceptionHelper.ThrowIfTrue(moviesWhereWillBeStoreActualFolder.Count() > 1, "Detected more than one movie where actual folder must be created.");

		var movieWithOriginalFolder = moviesWhereWillBeStoreActualFolder.First();
		var createMovieReleaseFolderResult = await _movieReleaseToFolderService.CreateAssociatedFolderAndFileAsync(movieRelease, movieWithOriginalFolder);
		if (createMovieReleaseFolderResult.IsFailure)
		{
			return Result.Failure<DiscId>(createMovieReleaseFolderResult.Error);
		}

		movieRelease.SetAssociatedFolder(createMovieReleaseFolderResult.Value);
		movieWithOriginalFolder.AddRelease(movieRelease);
		movies.Remove(movieWithOriginalFolder);              // foreach other movies we create a folder link instead real folder.

		foreach (var movie in movies) 
        {
            var settingLinkResult = await _movieReleaseToFolderService.CreateFolderLinkAsync(movieRelease, movie);
            if (settingLinkResult.IsFailure)
            {
                return Result.Failure(settingLinkResult.Error);
            }

            movie.AddRelease(movieRelease, releaseLinkPath: settingLinkResult.Value);
        }

        return Result.Success();
    }
}
