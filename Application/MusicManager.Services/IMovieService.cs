using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;

namespace MusicManager.Services
{
    public interface IMovieService
    {
        Task<Result> SaveFromFolder(IMovieFolder movieFolder, SongwriterId songwriterId);
    }
}