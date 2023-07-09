using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;

namespace MusicManager.Services
{
    public interface ICompilationService
    {
        Task<Result> SaveFromFolderAsync(ICompilationFolder compilationFolder, SongwriterId songwriterId, CancellationToken cancellationToken = default);
    }
}