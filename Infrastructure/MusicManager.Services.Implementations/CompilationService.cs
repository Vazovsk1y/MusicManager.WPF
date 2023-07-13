using MusicManager.Domain.Common;
using MusicManager.Domain.Entities;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services;
using MusicManager.Domain.Shared;
using MusicManager.Repositories;
using MusicManager.Repositories.Data;
using MusicManager.Services.Contracts.Base;

namespace MusicManager.Services.Implementations;

public class CompilationService : ICompilationService
{
    private readonly ISongService _songService;
    private readonly ISongwriterRepository _songwriterRepository;
    private readonly IPathToCompilationService _pathToCompilationService;
    private readonly IUnitOfWork _unitOfWork;

    public CompilationService(
        ISongService songService,
        ISongwriterRepository songwriterRepository,
        IPathToCompilationService pathToCompilationService,
        IUnitOfWork unitOfWork)
    {
        _songService = songService;
        _songwriterRepository = songwriterRepository;
        _pathToCompilationService = pathToCompilationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> SaveFromFolderAsync(DiscFolder compilationFolder, SongwriterId songwriterId, CancellationToken cancellationToken = default)
    {
        var compilationResult = await _pathToCompilationService
            .GetEntityAsync(compilationFolder.Path, songwriterId)
            .ConfigureAwait(false);

        if (compilationResult.IsFailure)
        {
            return compilationResult;
        }

        var compilation = compilationResult.Value;
        var songwriter = await _songwriterRepository.GetByIdWithCompilationsAsync(songwriterId, cancellationToken);
        if (songwriter is null) 
        {
            return Result.Failure(new Error("Songwriter with passed id is not exists in database."));
        }

        var addingResult = songwriter.AddCompilation(compilation, true);
        if (addingResult.IsFailure)
        {
            return addingResult;
        }

        foreach (var coverPath in compilationFolder.CoversPaths)
        {
            compilation.AddCover(coverPath);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var song in compilationFolder.Songs)
        {
            var result = await _songService.SaveFromFileInCompilationAsync(song, compilation.Id, cancellationToken);
            if (result.IsFailure)
            {
                return result;
            }
        }

        return Result.Success();
    }
}
