using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts.Base;

namespace MusicManager.Services
{
    public interface IMovieReleaseService
    {
        Task<Result> SaveFromFolderAsync(DiscFolder movieReleaseFolder, MovieId movieId, CancellationToken cancellationToken = default);
    }
}