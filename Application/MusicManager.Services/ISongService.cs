using MusicManager.Domain.Common;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;

namespace MusicManager.Services
{
    public interface ISongService
    {
        Task<Result> SaveFromFileInCompilationAsync(SongFile songFile, DiscId discId, CancellationToken cancellationToken = default);

        Task<Result> SaveFromFileInMovieReleaseAsync(SongFile songFile, DiscId discId, CancellationToken cancellationToken = default);
    }
}