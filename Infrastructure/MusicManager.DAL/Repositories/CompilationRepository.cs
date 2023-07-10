using MusicManager.Domain.Models;
using MusicManager.Repositories;
using MusicManager.Repositories.Data;

namespace MusicManager.DAL.Repositories;

public class CompilationRepository : BaseDiscRepository<Compilation>, ICompilationRepository
{
    public CompilationRepository(IApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
