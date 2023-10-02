using MusicManager.Domain.Common;
using MusicManager.Domain.Constants;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.ValueObjects;

public class CueInfo : ValueObject<CueInfo>
{
    public string CueFilePath { get; init; }

    public TimeSpan Index00 { get; init; }

    public TimeSpan Index01 { get; init; }

    public string SongNameInCue { get; init; }

    private CueInfo() { }

    internal static Result<CueInfo> Create(
        string cueFilePath, 
        TimeSpan index00, 
        TimeSpan index01, 
        string songNameInCueFile)
    {
        if (string.IsNullOrWhiteSpace(cueFilePath) || string.IsNullOrWhiteSpace(songNameInCueFile))
        {
            return Result.Failure<CueInfo>(DomainErrors.NullOrEmptyStringPassed());
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
            SongNameInCue = songNameInCueFile
        };
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return CueFilePath;
        yield return Index00;
        yield return Index01;
    }
}


