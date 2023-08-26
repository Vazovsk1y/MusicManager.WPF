using MusicManager.Domain.Common;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Dtos;

namespace MusicManager.Services
{
    public interface ISongService
    {
        Task<Result<IEnumerable<SongDTO>>> SaveFromFileAsync(SongFile songFile, DiscId discId, bool ignoreSongAddingResult, CancellationToken cancellationToken = default);

        Task<Result<IEnumerable<SongDTO>>> GetAllAsync(DiscId discId, CancellationToken cancellationToken = default);

        Task<Result<IEnumerable<SongDTO>>> SaveAsync(SongAddDTO songAddDTO, bool moveToParentFolder = true, CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(SongUpdateDTO songUpdateDTO, CancellationToken cancellationToken = default);
    }
}