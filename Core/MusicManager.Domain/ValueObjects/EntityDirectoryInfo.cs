using MusicManager.Domain.Common;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.ValueObjects;

public class EntityDirectoryInfo : ValueObject<EntityDirectoryInfo>
{
    public string Path { get; private set; } = null!;

    private EntityDirectoryInfo(string path)
    {
        Path = path;
    }

    public static Result<EntityDirectoryInfo> Create(string Path)
    {
        if (string.IsNullOrWhiteSpace(Path))
        {
            return Result.Failure<EntityDirectoryInfo>(DomainErrors.NullOrEmptyStringPassed(nameof(Path)));
        }

        return new EntityDirectoryInfo(Path);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Path;
    }
}
