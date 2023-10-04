using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Common;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;
using MusicManager.Repositories.Data;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Services.Extensions;

namespace MusicManager.Services.Implementations;

public class SongwriterService : ISongwriterService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IFolderToSongwriterService _pathToSongwriterService;
    private readonly IMovieService _movieService;
    private readonly ICompilationService _compilationService;
    private readonly ISongwriterToFolderService _songwriterToFolderService;
    private readonly IRoot _root;

	public SongwriterService(
		IFolderToSongwriterService pathToSongwriterService,
		IMovieService movieService,
		ICompilationService compilationService,
		IApplicationDbContext dbContext,
		ISongwriterToFolderService songwriterToFolderService,
		IRoot root)
	{
		_pathToSongwriterService = pathToSongwriterService;
		_movieService = movieService;
		_compilationService = compilationService;
		_dbContext = dbContext;
		_songwriterToFolderService = songwriterToFolderService;
		_root = root;
	}

	public async Task<Result> DeleteAsync(SongwriterId songwriterId, CancellationToken cancellationToken = default)
    {
        var songwriter = await _dbContext.Songwriters
            .Include(e => e.Movies)
            .Include(e => e.Compilations)
            .SingleOrDefaultAsync(e => e.Id == songwriterId, cancellationToken);

        if (songwriter is null)
        {
            return Result.Failure(ServicesErrors.SongwriterWithPassedIdIsNotExists());
        }

        var deletedMoviesIds = songwriter.Movies.Select(e => e.Id);
        var movieReleasesToRemove = _dbContext.MoviesReleases
            .Include(e => e.MoviesLinks)
            .Where(Filter);

        _dbContext.Songwriters.Remove(songwriter);
        _dbContext.Discs.RemoveRange(movieReleasesToRemove);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();

        bool Filter(MovieRelease movieRelease)
        {
            var moviesIds = movieRelease.MoviesLinks.Select(e => e.MovieId);
            bool mainCondition = moviesIds.Any(e => deletedMoviesIds.Contains(e)) && movieRelease.MoviesLinks.Count == 1;
            bool secondaryCondition = moviesIds.All(e => deletedMoviesIds.Contains(e));

            return mainCondition || secondaryCondition;
        }
    }

    public async Task<Result<IReadOnlyCollection<SongwriterDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var songwriters = await _dbContext
            .Songwriters
            .AsNoTracking()
            .Select(e => e.ToDTO())
            .ToListAsync(cancellationToken);

        return songwriters;
    }

    public async Task<Result<IReadOnlyCollection<SongwriterLookupDTO>>> GetLookupsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Songwriters
            .AsNoTracking()
            .Select(sngw => sngw.ToLookupDTO())
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<SongwriterId>> SaveAsync(SongwriterAddDTO songwriterAddDTO, bool createAssociatedFolder = true, CancellationToken cancellationToken = default)
    {
        var songwriterCreationResult = Songwriter.Create(songwriterAddDTO.Name, songwriterAddDTO.LastName);

        if (songwriterCreationResult.IsFailure)
        {
            return Result.Failure<SongwriterId>(songwriterCreationResult.Error);
        }

        var createdSongwriter = songwriterCreationResult.Value;
        if (createAssociatedFolder)
        {
            var creatingAssociatedFolderAndFileResult = await _songwriterToFolderService.CreateAssociatedFolderAndFileAsync(createdSongwriter);
            if (creatingAssociatedFolderAndFileResult.IsFailure)
            {
                return Result.Failure<SongwriterId>(creatingAssociatedFolderAndFileResult.Error);
            }
            createdSongwriter.SetAssociatedFolder(creatingAssociatedFolderAndFileResult.Value);
        }

        await _dbContext.Songwriters.AddAsync(createdSongwriter, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return createdSongwriter.Id;
    }

    public async Task<Result<SongwriterDTO>> SaveFromFolderAsync(SongwriterFolder songwriterFolder, CancellationToken cancellationToken = default)
    {
        var songwriterCreationResult = await _pathToSongwriterService
            .GetEntityAsync(songwriterFolder.Path)
            .ConfigureAwait(false);

        if (songwriterCreationResult.IsFailure)
        {
            return Result.Failure<SongwriterDTO>(songwriterCreationResult.Error);
        }

        var songwriter = songwriterCreationResult.Value;
        if (_dbContext.Songwriters.Any(e => e.AssociatedFolderInfo == EntityDirectoryInfo.Create(songwriterFolder.Path.GetRelational(_root)).Value))
        {
            return Result.Failure<SongwriterDTO>(new Error("Songwriter with passed directory path is already exists."));
        }

        await _dbContext.Songwriters.AddAsync(songwriter, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var moviesDtos = new List<MovieDTO>();
        var compilationsDtos = new List<CompilationDTO>();

        foreach (var movieFolder in songwriterFolder.MoviesFolders)
        {
            var movieResult = await _movieService.SaveFromFolderAsync(movieFolder, songwriter.Id, cancellationToken);

            if (movieResult.IsFailure)
            {
                return Result.Failure<SongwriterDTO>(movieResult.Error);
            }

            moviesDtos.Add(movieResult.Value);
        }

        foreach (var compilationFolder in songwriterFolder.CompilationsFolders)
        {
            var compilationResult = await _compilationService.SaveFromFolderAsync(compilationFolder, songwriter.Id, cancellationToken);

            if (compilationResult.IsFailure)
            {
                return Result.Failure<SongwriterDTO>(compilationResult.Error);
            }

            compilationsDtos.Add(compilationResult.Value);
        }

        return songwriter.ToDTO() with
        {
        };
    }
}
