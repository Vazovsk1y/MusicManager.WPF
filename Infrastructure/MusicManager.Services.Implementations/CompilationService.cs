using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Common;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services;
using MusicManager.Domain.Shared;
using MusicManager.Repositories.Data;
using MusicManager.Services.Contracts.Base;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Services.Extensions;

namespace MusicManager.Services.Implementations;

public class CompilationService : ICompilationService
{
    private readonly ISongService _songService;
    private readonly IPathToCompilationService _pathToCompilationService;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICompilationToFolderService _compilationToFolderService;

    public CompilationService(
        ISongService songService,
        IPathToCompilationService pathToCompilationService,
        IApplicationDbContext dbContext,
        ICompilationToFolderService compilationToFolderService)
    {
        _songService = songService;
        _pathToCompilationService = pathToCompilationService;
        _dbContext = dbContext;
        _compilationToFolderService = compilationToFolderService;
    }

    public async Task<Result<IEnumerable<CompilationDTO>>> GetAllAsync(SongwriterId songwriterId, CancellationToken cancellation = default)
    {
        var result = new List<CompilationDTO>();
        var songwriter = await _dbContext
            .Songwriters
            .AsNoTracking()
            .Include(e => e.Compilations)
            .SingleOrDefaultAsync(e => e.Id == songwriterId, cancellation);

        if (songwriter is null)
        {
            return Result.Failure<IEnumerable<CompilationDTO>>(ServicesErrors.SongwriterWithPassedIdIsNotExists());
        }

        var compilations = songwriter.Compilations;
        foreach (var compilation in compilations)
        {
            var songsResult = await _songService.GetAllAsync(compilation.Id, cancellation);
            if (songsResult.IsFailure)
            {
                return Result.Failure<IEnumerable<CompilationDTO>>(songsResult.Error);
            }

            result.Add(compilation.ToDTO() with
            {
                SongDTOs = songsResult.Value
            });
        }

        return result;
    }

    public async Task<Result<DiscId>> SaveAsync(CompilationAddDTO compilationAddDTO, CancellationToken cancellationToken = default)
    {
        var songwriter = await _dbContext
            .Songwriters
            .Include(e => e.Compilations)
            .SingleOrDefaultAsync(e => e.Id == compilationAddDTO.SongwriterId, cancellationToken);

        if (songwriter is null)
        {
            return Result.Failure<DiscId>(ServicesErrors.SongwriterWithPassedIdIsNotExists());
        }

        var creationResult = Compilation.Create(
            compilationAddDTO.SongwriterId,
            compilationAddDTO.DiscType,
            compilationAddDTO.Identifier
            );

        if (creationResult.IsFailure) 
        {
            return Result.Failure<DiscId>(creationResult.Error);
        }

        var compilation = creationResult.Value;
        var addingResult = songwriter.AddCompilation(compilation, true);
        if (addingResult.IsFailure)
        {
            return Result.Failure<DiscId>(addingResult.Error);
        }

        var createdAssociatedFolderAndFileResult = await _compilationToFolderService.CreateAssociatedFolderAndFileAsync(compilation, songwriter);
        if (createdAssociatedFolderAndFileResult.IsFailure)
        {
            return Result.Failure<DiscId>(createdAssociatedFolderAndFileResult.Error);
        }

        compilation.SetDirectoryInfo(createdAssociatedFolderAndFileResult.Value);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(compilation.Id);
    }

    public async Task<Result<CompilationDTO>> SaveFromFolderAsync(DiscFolder compilationFolder, SongwriterId songwriterId, CancellationToken cancellationToken = default)
    {
        var compilationResult = await _pathToCompilationService
            .GetEntityAsync(compilationFolder.Path, songwriterId)
            .ConfigureAwait(false);

        if (compilationResult.IsFailure)
        {
            return Result.Failure<CompilationDTO>(compilationResult.Error);
        }

        var compilation = compilationResult.Value;
        var songwriter = await _dbContext
            .Songwriters
            .Include(e => e.Compilations)
            .SingleOrDefaultAsync(e => e.Id == songwriterId, cancellationToken);

        if (songwriter is null) 
        {
            return Result.Failure<CompilationDTO>(ServicesErrors.SongwriterWithPassedIdIsNotExists());
        }

        var addingResult = songwriter.AddCompilation(compilation, true);
        if (addingResult.IsFailure)
        {
            return Result.Failure<CompilationDTO>(addingResult.Error);
        }

        foreach (var coverPath in compilationFolder.CoversPaths)
        {
            compilation.AddCover(coverPath);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var songsDtos = new List<SongDTO>();
        foreach (var song in compilationFolder.Songs)
        {
            var result = await _songService.SaveFromFileAsync(song, compilation.Id, true, cancellationToken);
            if (result.IsFailure)
            {
                return Result.Failure<CompilationDTO>(result.Error);
            }

            songsDtos.AddRange(result.Value);
        }

        return compilation.ToDTO() with
        {
            SongDTOs = songsDtos,
        };
    }
}
