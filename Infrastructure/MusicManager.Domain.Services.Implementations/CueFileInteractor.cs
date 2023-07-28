using MusicManager.Domain.Constants;
using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Services.Implementations.Extensions;
using MusicManager.Domain.Shared;
using MusicManager.Utils;
using System.Text.RegularExpressions;

namespace MusicManager.Domain.Services.Implementations;

public partial class CueFileInteractor : ICueFileInteractor
{
    #region --Fields--

    #region --Constants--

    private const string TrackKeyWord = "TRACK";
    private const string TitleKeyWord = "TITLE";
    private const string PerformerKeyWord = "PERFORMER";
    private const string ISRCKeyWord = "ISRC";
    private const string Index00KeyWord = "INDEX 00";
    private const string Index01KeyWord = "INDEX 01";

    #endregion

    [GeneratedRegex("\\d{2}:\\d{2}:\\d{2}")]
    private static partial Regex GetTimeSpanRowFromLine();

    private static readonly char[] _cueRowsSeparators = new char[]
    {
        '\r',
        '\n',
    };

    #endregion

    #region --Properties--



    #endregion

    #region --Constructors--



    #endregion

    #region --Methods--

    #region --Public--

    public async Task<Result<IEnumerable<ICueFileTrack>>> GetTracksAsync(string cueFilePath, CancellationToken cancellationToken = default)
    {
        var result = IsAbleToReadFile(cueFilePath);
        if (result.IsFailure)
        {
            return Result.Failure<IEnumerable<ICueFileTrack>>(result.Error);
        }

        var fileInfo = result.Value;
        string cueFileText = await GetCueFileTextAsync(fileInfo, cancellationToken).ConfigureAwait(false);
        var tracksSections = cueFileText
            .Split(TrackKeyWord, StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)                                                     // skip the row that before first track section.
            .ToList();                                                   

        if (tracksSections.Count is 0)
        {
            return Result.Failure<IEnumerable<ICueFileTrack>>(new Error("No tracks were founded in cue file."));
        }

        var tracks = new List<ICueFileTrack>();
        foreach (var section in tracksSections)
        {
            tracks.Add(GetTrackFromSection(section));
        }

        return tracks;
    }

    #endregion

    #region --Private--

    private Result<FileInfo> IsAbleToReadFile(string cuePath)
    {
        var fileInfo = new FileInfo(cuePath);

        if (!PathValidator.IsValid(cuePath))
        {
            return Result.Failure<FileInfo>(DomainServicesErrors.PassedFilePathIsInvalid(cuePath));
        }

        if (!fileInfo.Exists)
        {
            return Result.Failure<FileInfo>(DomainServicesErrors.PassedFileIsNotExists(cuePath));
        }

        if (fileInfo.Extension != DomainConstants.CueExtension)
        {
            return Result.Failure<FileInfo>(new Error($"Passed [{cuePath}] file is not with cue extension."));
        }

        return fileInfo;
    }

    private async Task<string> GetCueFileTextAsync(FileInfo fileInfo, CancellationToken cancellationToken)
    {
        using var streamReader = new StreamReader(fileInfo.OpenRead());
        string cueFileText = await streamReader.ReadToEndAsync(cancellationToken);
        return cueFileText;
    }

    private CueFileTrack GetTrackFromSection(string trackSection)
    {
        string[] trackSectionRows = trackSection.Split(_cueRowsSeparators, StringSplitOptions.RemoveEmptyEntries);
        var track = new CueFileTrack();

        foreach (var row in trackSectionRows)
        {
            if (row.Contains(TitleKeyWord))
            {
                track.Title = GetRowFromQuotes(row) ?? "Undefined";
                continue;
            }

            if (row.Contains(PerformerKeyWord))
            {
                track.Performer = GetRowFromQuotes(row) ?? "Undefined";
                continue;
            }

            if (row.Contains(ISRCKeyWord))
            {
                track.Isrc = row
                   .RemoveAllSpaces()
                   .Trim(ISRCKeyWord.ToArray());
                continue;
            }

            if (row.Contains(Index00KeyWord))
            {
                var match = GetTimeSpanRowFromLine().Match(row);
                if (match.Success)
                {
                    track.Index00 = ParseToTimeSpan(match.Value);
                }
                continue;
            }

            if (row.Contains(Index01KeyWord))
            {
                var match = GetTimeSpanRowFromLine().Match(row);
                if (match.Success)
                {
                    track.Index01 = ParseToTimeSpan(match.Value);
                }
                break;
            }
        }

        return track;
    }

    private TimeSpan ParseToTimeSpan(string timeSpanString)
    {
        var timeComponents = timeSpanString.Split(":");
        const int IndexesComponentsCount = 3;

        if (timeComponents.Length != IndexesComponentsCount)
        {
            return TimeSpan.Zero;
        }

        bool parsingMinutesResult = int.TryParse(timeComponents[0], out int minutes);
        bool parsingSecondsResult = int.TryParse(timeComponents[1], out int seconds);
        bool parsingMillisecondsResult = int.TryParse(timeComponents[2], out int milliseconds);

        if (!parsingMinutesResult || !parsingSecondsResult || !parsingMillisecondsResult)
        {
            return TimeSpan.Zero;
        }

        return new TimeSpan(0, 0, minutes, seconds, milliseconds);
    }

    private string? GetRowFromQuotes(string rowWithQuotes)
    {
        int firstQuoteIndex = rowWithQuotes.IndexOf('"');
        if (firstQuoteIndex != -1)
        {
            int secondQuoteIndex = rowWithQuotes.IndexOf('"', firstQuoteIndex + 1);
            if (secondQuoteIndex != -1)
            {
               return rowWithQuotes.Substring(firstQuoteIndex + 1, secondQuoteIndex - firstQuoteIndex - 1);
            }
        }
        return null; 
    }

    #endregion

    #endregion
}