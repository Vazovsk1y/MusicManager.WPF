using MusicManager.Domain.Common;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Dtos;

namespace MusicManager.Services
{
    public interface ISongService
    {
        Task<Result<IEnumerable<SongDTO>>> SaveFromFileAsync(SongFile songFile, DiscId discId, CancellationToken cancellationToken = default);

        Task<Result<IEnumerable<SongDTO>>> GetAllAsync(DiscId discId, CancellationToken cancellationToken = default);
    }
}