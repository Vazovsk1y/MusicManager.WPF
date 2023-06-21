using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.ValueObjects;

public class EntityDirectoryInfo
{
    public string Name { get; private set; } = string.Empty;

    public string FullPath { get; private set; } = string.Empty;

    private EntityDirectoryInfo(string name, string fullPath)
    {
        Name = name;
        FullPath = fullPath;
    }

    public static Result<EntityDirectoryInfo> Create(string name, string fullPath)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(fullPath))
        {
            return Result.Failure<EntityDirectoryInfo>(DomainErrors.NullOrEmptyStringPassedError());
        }

        if (!fullPath.EndsWith(name))
        {
            return Result.Failure<EntityDirectoryInfo>(DomainErrors.EntityDirectoryInfoErrors.IncorrectPathPassed); 
        }

        return new EntityDirectoryInfo(name, fullPath);
    }
}
