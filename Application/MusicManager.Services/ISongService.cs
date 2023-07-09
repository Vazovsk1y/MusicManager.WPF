using MusicManager.Domain.Common;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;

namespace MusicManager.Services
{
    public interface ISongService
    {
        Task<Result> SaveFromFileInCompilationAsync(ISongFile songFile, DiscId discId, CancellationToken cancellationToken = default);

        Task<Result> SaveFromFileInMovieReleaseAsync(ISongFile songFile, DiscId discId, CancellationToken cancellationToken = default);
    }
}