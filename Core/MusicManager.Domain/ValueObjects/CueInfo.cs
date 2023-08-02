using MusicManager.Domain.Common;
using MusicManager.Domain.Constants;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.ValueObjects;

public class CueInfo : ValueObject<CueInfo>
{
    public string CueFilePath { get; private set; }

    public TimeSpan Index00 { get; private set; }

    public TimeSpan Index01 { get; private set; }

    private CueInfo() { }

    internal static Result<CueInfo> Create(string cueFilePath, TimeSpan index00, TimeSpan index01)
    {
        if (string.IsNullOrWhiteSpace(cueFilePath))
        {
            return Result.Failure<CueInfo>(DomainErrors.NullOrEmptyStringPassed(nameof(cueFilePath)));
        }

        if (!cueFilePath.EndsWith(DomainConstants.CueExtension))
        {
            return Result.Failure<CueInfo>(DomainErrors.SongPlayInfo.IncorrectCuePathPassed(cueFilePath));
        }

        return new CueInfo()
        {
            CueFilePath = cueFilePath,
            Index00 = index00,
            Index01 = index01,
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CueFilePath;
        yield return Index00;
        yield return Index01;
    }
}


