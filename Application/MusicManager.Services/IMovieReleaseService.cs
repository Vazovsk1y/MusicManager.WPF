using MusicManager.Domain.Common;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts.Base;
using MusicManager.Services.Contracts.Dtos;

namespace MusicManager.Services
{
    public interface IMovieReleaseService
    {
        Task<Result<MovieReleaseDTO>> SaveFromFolderAsync(DiscFolder movieReleaseFolder, MovieId movieId, CancellationToken cancellationToken = default);

        Task<Result<IEnumerable<MovieReleaseDTO>>> GetAllAsync(MovieId movieId, CancellationToken cancellationToken = default);

        Task<Result<DiscId>> SaveAsync(MovieReleaseAddDTO movieReleaseAddDTO, bool createAssociatedFolder = true, CancellationToken cancellationToken = default);
    }
}