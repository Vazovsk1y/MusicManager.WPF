using MusicManager.Domain.Services;

namespace MusicManager.Domain.Extensions;

public static class StringExtensions
{
    public static string RemoveAllSpaces(this string row) => row.Replace(" ", string.Empty);

    public static string GetRelational(this string fullPath, IRoot relationalBy)
    {
        if (relationalBy.RootPath is null || !fullPath.StartsWith(relationalBy.RootPath)) 
        {
            return fullPath;
        }

        return fullPath.Replace($"{relationalBy.RootPath}\\", string.Empty);
    }
}
