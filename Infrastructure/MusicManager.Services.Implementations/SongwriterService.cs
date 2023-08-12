﻿using Microsoft.EntityFrameworkCore;
﻿using MusicManager.Domain.Models;
using MusicManager.Domain.Services;
using MusicManager.Domain.Shared;
using MusicManager.Repositories;
using MusicManager.Repositories.Data;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Services.Extensions;

namespace MusicManager.Services.Implementations;

public class SongwriterService : ISongwriterService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPathToSongwriterService _pathToSongwriterService;
    private readonly IMovieService _movieService;
    private readonly ICompilationService _compilationService;

    public SongwriterService(
        ISongwriterRepository songwriterRepository,
        IUnitOfWork unitOfWork,
        IPathToSongwriterService pathToSongwriterService,
        IMovieService movieService,
        ICompilationService compilationService,
        IApplicationDbContext dbContext)
    {
        _songwriterRepository = songwriterRepository;
        _unitOfWork = unitOfWork;
        _pathToSongwriterService = pathToSongwriterService;
        _movieService = movieService;
        _compilationService = compilationService;
        _dbContext = dbContext;
    }

    public async Task<Result<IEnumerable<SongwriterDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = new List<SongwriterDTO>();
        var songwriters = await _dbContext
            .Songwriters
            .AsNoTracking()
            .ToListAsync(cancellationToken);

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
        return await _dbContext
            .Songwriters
            .AsNoTracking()
            .Select(sngw => sngw.ToLookupDTO())
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<SongwriterId>> SaveAsync(SongwriterAddDTO songwriterAddDTO, CancellationToken cancellationToken = default)
    {
        var songwriterCreationResult = Songwriter.Create(songwriterAddDTO.Name, songwriterAddDTO.LastName);

        if (songwriterCreationResult.IsFailure)
        {
            return Result.Failure<SongwriterId>(songwriterCreationResult.Error);
        }

        var createdSongwriter = songwriterCreationResult.Value;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return songwriterCreationResult.Value.Id;
        await _dbContext.Songwriters.AddAsync(createdSongwriter, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return createdSongwriter.Id;
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
        if (_dbContext.Songwriters.IsSongwriterWithPassedEntityDirectoryInfoExists(songwriter.EntityDirectoryInfo))
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
            CompilationDTOs = compilationsDtos,
            MovieDTOs = moviesDtos,
        };
    }
}
