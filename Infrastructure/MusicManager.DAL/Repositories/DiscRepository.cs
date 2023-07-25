using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Common;
using MusicManager.Repositories.Common;
using MusicManager.Repositories.Data;

namespace MusicManager.DAL.Repositories;

public class DiscRepository : BaseDiscRepository<Disc>
{
    public DiscRepository(IApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
