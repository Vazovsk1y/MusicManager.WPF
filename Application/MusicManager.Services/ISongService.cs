using MusicManager.Domain.Common;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Dtos;

namespace MusicManager.Services;

public interface ISongService
{
    Task<Result<IReadOnlyCollection<SongDTO>>> SaveFromFileAsync(SongFile songFile, DiscId discId, bool ignoreSongAddingResult, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<SongDTO>>> GetAllAsync(DiscId parentId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<SongDTO>>> SaveAsync(SongAddDTO songAddDTO, bool moveToParentFolder = true, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(SongUpdateDTO songUpdateDTO, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(SongId songId, CancellationToken cancellationToken = default);
}