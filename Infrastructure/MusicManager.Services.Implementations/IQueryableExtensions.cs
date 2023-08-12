using MusicManager.Domain.Models;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Services.Implementations;

public static class IQueryableExtensions
{
    public static bool IsSongwriterWithPassedEntityDirectoryInfoExists(this IQueryable<Songwriter> songwriters, EntityDirectoryInfo? entityDirectoryInfo)
    {
        return songwriters.Any(e => e.EntityDirectoryInfo == entityDirectoryInfo);
    }
}


