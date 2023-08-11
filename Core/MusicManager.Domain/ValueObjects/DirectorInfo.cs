using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.ValueObjects;

public class DirectorInfo
{
    public string Surname { get; init; }

    public string Name { get; init; } 

    private DirectorInfo() { }

    public static Result<DirectorInfo> Create(string name, string surname)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(surname))
        {
            return Result.Failure<DirectorInfo>(DomainErrors.NullOrEmptyStringPassed());
        }

        return new DirectorInfo
        {
            Name = name,
            Surname = surname
        };
    }
}

