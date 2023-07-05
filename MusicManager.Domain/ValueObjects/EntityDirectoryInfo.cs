using MusicManager.Domain.Common;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.ValueObjects;

public class EntityDirectoryInfo : ValueObject<EntityDirectoryInfo>
{
    public string Name { get; }

    public string FullPath { get; private set; } = string.Empty;

    private EntityDirectoryInfo(string fullPath)
    {
        FullPath = fullPath;
        Name = Path.GetFileName(fullPath);
    }

    public static Result<EntityDirectoryInfo> Create(string fullPath)
    {
        if (string.IsNullOrWhiteSpace(fullPath))
        {
            return Result.Failure<EntityDirectoryInfo>(DomainErrors.NullOrEmptyStringPassedError(nameof(fullPath)));
        }

        return new EntityDirectoryInfo(fullPath);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return FullPath;
    }
}
