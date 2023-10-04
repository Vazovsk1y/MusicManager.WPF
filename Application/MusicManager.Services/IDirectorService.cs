using MusicManager.Domain.Entities;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts.Dtos;

namespace MusicManager.Services;

public interface IDirectorService
{
    Task<Result<IReadOnlyCollection<DirectorDTO>>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Result<DirectorId>> SaveAsync(string fullName, CancellationToken cancellationToken = default);
}
