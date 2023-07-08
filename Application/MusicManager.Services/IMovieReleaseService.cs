using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;

namespace MusicManager.Services
{
    public interface IMovieReleaseService
    {
        Task<Result> SaveFromFolder(IMovieReleaseFolder movieReleaseFolder, MovieId movieId);
    }
}