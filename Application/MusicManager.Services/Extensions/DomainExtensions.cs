using MusicManager.Domain.ValueObjects;

namespace MusicManager.Services.Extensions;

public static class DomainExtensions
{
    public static string GetParentPath(this EntityDirectoryInfo entityDirectoryInfo)
    {
        var result = Path.GetDirectoryName(entityDirectoryInfo.FullPath);

        ArgumentNullException.ThrowIfNull(result);

        return result;
    }
}