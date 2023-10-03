using MusicManager.Domain.Common;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.ValueObjects;

public class EntityDirectoryInfo : ValueObject<EntityDirectoryInfo>
{
    public string Path { get; private set; }

    private EntityDirectoryInfo(string path)
    {
        Path = path;
    }

    public static Result<EntityDirectoryInfo> Create(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return Result.Failure<EntityDirectoryInfo>(DomainErrors.NullOrEmptyStringPassed(nameof(path)));
        }

        return new EntityDirectoryInfo(path);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Path;
    }
}
