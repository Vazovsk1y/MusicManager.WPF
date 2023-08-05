using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Dtos;

namespace MusicManager.Services
{
    public interface IMovieService
    {
        Task<Result<MovieDTO>> SaveFromFolderAsync(MovieFolder movieFolder, SongwriterId songwriterId, CancellationToken cancellationToken = default);

        Task<Result<IEnumerable<MovieDTO>>> GetAllAsync(SongwriterId songwriterId, CancellationToken cancellation = default);

        Task<Result<MovieId>> SaveAsync(MovieAddDTO movieAddDTO, CancellationToken cancellationToken = default);
    }
}