using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts.Base;

namespace MusicManager.Services
{
    public interface IMovieReleaseService
    {
        Task<Result> SaveFromFolderAsync(IDiscFolder movieReleaseFolder, MovieId movieId, CancellationToken cancellationToken = default);
    }
}