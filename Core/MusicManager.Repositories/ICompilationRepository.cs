using MusicManager.Domain.Models;
using MusicManager.Repositories.Common;

namespace MusicManager.Repositories;

public interface ICompilationRepository : IDiscRepository<Compilation>
{
}

