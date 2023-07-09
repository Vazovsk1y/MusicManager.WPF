using MusicManager.Domain.Models;
using MusicManager.Repositories.Common;

namespace MusicManager.Repositories;

public interface IMovieReleaseRepository : IDiscRepository<MovieRelease>
{
}
