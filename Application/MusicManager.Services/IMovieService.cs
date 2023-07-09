using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;

namespace MusicManager.Services
{
    public interface IMovieService
    {
        Task<Result> SaveFromFolderAsync(IMovieFolder movieFolder, SongwriterId songwriterId, CancellationToken cancellationToken = default);
    }
}