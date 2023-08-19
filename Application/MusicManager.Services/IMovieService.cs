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

        Task<Result<IEnumerable<MovieLookupDTO>>> GetLookupsAsync(CancellationToken cancellationToken = default);

        Task<Result<MovieId>> SaveAsync(MovieAddDTO movieAddDTO, bool createAssociatedFolder = true, CancellationToken cancellationToken = default);

        Task<Result> AddExistingMovieRelease(ExistingMovieReleaseToMovieDTO dto, CancellationToken cancellationToken = default);
    }
}