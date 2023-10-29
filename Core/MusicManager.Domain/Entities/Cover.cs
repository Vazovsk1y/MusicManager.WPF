using MusicManager.Domain.Common;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Entities;

public class Cover
{
    public CoverId Id { get; }

    public DiscId DiscId { get; private set; }

    public string Path { get; private set; }

#pragma warning disable CS8618 
	private Cover()
	{
        Id = CoverId.Create();
    }
#pragma warning restore CS8618 

	internal static Result<Cover> Create(DiscId discId, string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return Result.Failure<Cover>(DomainErrors.NullOrEmptyStringPassed("cover path"));
        }

        return new Cover() 
        {
            Path = path,
            DiscId = discId
        };
    }
}

public record CoverId(Guid Value)
{
    public static CoverId Create() => new(Guid.NewGuid());
}
