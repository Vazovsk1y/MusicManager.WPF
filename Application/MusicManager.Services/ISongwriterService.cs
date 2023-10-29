using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Dtos;

namespace MusicManager.Services;

public interface ISongwriterService
{
    Task<Result<SongwriterDTO>> SaveFromFolderAsync(SongwriterFolder songwriterFolder, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<SongwriterDTO>>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<SongwriterLookupDTO>>> GetLookupsAsync(CancellationToken cancellationToken= default);

    Task<Result<SongwriterId>> SaveAsync(SongwriterAddDTO songwriterAddDTO, bool createAssociatedFolder = true, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(SongwriterId songwriterId, CancellationToken cancellationToken = default);
}