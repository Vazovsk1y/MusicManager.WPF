using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Dtos;

namespace MusicManager.Services;

public interface IMovieService
{
    Task<Result<MovieDTO>> SaveFromFolderAsync(MovieFolder movieFolder, SongwriterId songwriterId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<MovieDTO>>> GetAllAsync(SongwriterId parentId, CancellationToken cancellation = default);

    Task<Result<IReadOnlyCollection<MovieLookupDTO>>> GetLookupsAsync(CancellationToken cancellationToken = default);

    Task<Result<MovieId>> SaveAsync(MovieAddDTO movieAddDTO, bool createAssociatedFolder = true, CancellationToken cancellationToken = default);

    Task<Result> AddReleaseAsync(MovieReleaseMovieDTO relationDto, bool createAssociatedLink = true, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(MovieUpdateDTO movieUpdateDTO, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(MovieId movieId, CancellationToken cancellationToken = default);
}