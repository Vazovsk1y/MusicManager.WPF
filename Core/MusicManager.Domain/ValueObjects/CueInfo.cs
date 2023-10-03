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

    public string SongTitleInCue { get; init; }

#pragma warning disable CS8618
	private CueInfo() { }

#pragma warning restore CS8618

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

        if (!cueFilePath.EndsWith(DomainConstants.CueExtension, StringComparison.OrdinalIgnoreCase))
        {
            return Result.Failure<CueInfo>(DomainErrors.PlaybackInfo.IncorrectCuePathPassed(cueFilePath));
        }

        return new CueInfo()
        {
            CueFilePath = cueFilePath,
            Index00 = index00,
            Index01 = index01,
            SongTitleInCue = songNameInCueFile
        };
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return CueFilePath;
        yield return Index00;
        yield return Index01;
    }
}


