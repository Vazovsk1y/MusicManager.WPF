using MusicManager.Domain.Common;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts.Base;
using MusicManager.Services.Contracts.Dtos;

namespace MusicManager.Services;

public interface ICompilationService
{
    Task<Result<CompilationDTO>> SaveFromFolderAsync(DiscFolder compilationFolder, SongwriterId songwriterId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<CompilationDTO>>> GetAllAsync(SongwriterId parentId, CancellationToken cancellation = default);

    Task<Result<DiscId>> SaveAsync(CompilationAddDTO compilationAddDTO, bool createAssociatedFolder = true, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(CompilationUpdateDTO compilationUpdateDTO, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(DiscId discId, CancellationToken cancellationToken = default);
}