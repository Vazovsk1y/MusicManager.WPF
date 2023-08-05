using MusicManager.Domain.Services;
using MusicManager.Domain.Shared;
using MusicManager.Repositories;
using MusicManager.Repositories.Data;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Services.Mappers;

namespace MusicManager.Services.Implementations;

public class SongwriterService : ISongwriterService
{
    private readonly ISongwriterRepository _songwriterRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPathToSongwriterService _pathToSongwriterService;
    private readonly IMovieService _movieService;
    private readonly ICompilationService _compilationService;

    public SongwriterService(
        ISongwriterRepository songwriterRepository,
        IUnitOfWork unitOfWork,
        IPathToSongwriterService pathToSongwriterService,
        IMovieService movieService,
        ICompilationService compilationService)
    {
        _songwriterRepository = songwriterRepository;
        _unitOfWork = unitOfWork;
        _pathToSongwriterService = pathToSongwriterService;
        _movieService = movieService;
        _compilationService = compilationService;
    }

    public async Task<Result<IEnumerable<SongwriterDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = new List<SongwriterDTO>();
        var songwriters = await _songwriterRepository.LoadAllAsync(cancellationToken);

        foreach (var songwriter in songwriters)
        {
            var moviesResult = await _movieService.GetAllAsync(songwriter.Id, cancellationToken);
            if (moviesResult.IsFailure)
            {
                return Result.Failure<IEnumerable<SongwriterDTO>>(moviesResult.Error);
            }

            var compilationsResult = await _compilationService.GetAllAsync(songwriter.Id, cancellationToken);
            if (compilationsResult.IsFailure)
            {
                return Result.Failure<IEnumerable<SongwriterDTO>>(compilationsResult.Error);
            }

            result.Add(songwriter.ToDTO() with
            {
                MovieDTOs = moviesResult.Value,
                CompilationDTOs = compilationsResult.Value
            });
        }

        return result;
    }

    public async Task<Result<IEnumerable<SongwriterLookupDTO>>> GetLookupsAsync(CancellationToken cancellationToken = default)
    {
        var songwriters = await _songwriterRepository.LoadAllAsync(cancellationToken);
        return songwriters.Select(sngw => sngw.ToLookupDTO()).ToList();
    }

    public async Task<Result<SongwriterDTO>> SaveFromFolderAsync(SongwriterFolder songwriterFolder, CancellationToken cancellationToken = default)
    {
        var songWriterResult = await _pathToSongwriterService
            .GetEntityAsync(songwriterFolder.Path)
            .ConfigureAwait(false);

        if (songWriterResult.IsFailure)
        {
            return Result.Failure<SongwriterDTO>(songWriterResult.Error);
        }

        var songwriter = songWriterResult.Value;
        if (await _songwriterRepository.IsExistsWithPassedDirectoryInfo(songwriter.EntityDirectoryInfo!))
        {
            return Result.Failure<SongwriterDTO>(new Error("Songwriter with passed directory path is already exists."));
        }

        await _songwriterRepository.InsertAsync(songwriter, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
            CompilationDTOs = compilationsDtos,
            MovieDTOs = moviesDtos,
        };
    }
}


