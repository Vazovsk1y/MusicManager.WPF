using MusicManager.Domain.Common;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts.Base;
using MusicManager.Services.Contracts.Dtos;

namespace MusicManager.Services
{
    public interface ICompilationService
    {
        Task<Result<CompilationDTO>> SaveFromFolderAsync(DiscFolder compilationFolder, SongwriterId songwriterId, CancellationToken cancellationToken = default);

        Task<Result<IEnumerable<CompilationDTO>>> GetAllAsync(SongwriterId songwriterId, CancellationToken cancellation = default);

        Task<Result<DiscId>> SaveAsync(CompilationAddDTO compilationAddDTO, bool createAssociatedFolder = true, CancellationToken cancellationToken = default);
    }
}