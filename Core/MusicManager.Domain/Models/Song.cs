using MusicManager.Domain.Common;
using MusicManager.Domain.Entities;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Models;

public class Song : IAggregateRoot
{
    #region --Fields--



    #endregion

    #region --Properties--

    public SongId Id { get; private set; }

    public DiscId DiscId { get; private set; }

    public DiscNumber? DiscNumber { get; private set; }

    public PlaybackInfo PlaybackInfo { get; private set; }

    public string Title { get; private set; }

    public int Order { get; private set; }

    public bool IsFromCue => PlaybackInfo.CueInfo is not null;

	#endregion

	#region --Constructors--

#pragma warning disable CS8618
	private Song(string title, DiscId discId)
	{
        Id = SongId.Create();
        DiscId = discId;
        Title = title;
    }

#pragma warning restore CS8618

	#endregion

	#region --Methods--

    private static Result<Song> Create(
        DiscId discId, 
        string title,
        int order,
        DiscNumber? discNumber = null)
	{
		if (string.IsNullOrWhiteSpace(title))
		{
			return Result.Failure<Song>(DomainErrors.NullOrEmptyStringPassed(nameof(title)));
		}

		if (int.IsNegative(order))
		{
			return Result.Failure<Song>(DomainErrors.Song.SongOrderCouldNotBeNegativeNumber);
		}

		var song = new Song(title, discId)
		{
			Order = order
		};

		song.DiscNumber = discNumber;
		return song;
	}

	public static Result<Song> Create(
        DiscId discId, 
        string title,
        int order,
        string executableFilePath,
        TimeSpan duration,
		DiscNumber? discNumber = null)
    {
        var songCreationResult = Create(discId, title, order, discNumber);

        if (songCreationResult.IsFailure)
        {
            return songCreationResult;
        }

        var song = songCreationResult.Value;
        var settingInfoResult = song.SetPlaybackInfo(executableFilePath, duration);

        if (settingInfoResult.IsFailure)
        {
            return Result.Failure<Song>(settingInfoResult.Error);
        }

        return song;
    }

    public static Result<Song> Create(
        DiscId discId, 
        string title,
        int order,
        string executableFilePath,
        TimeSpan duration,
        string cueFilePath,
        TimeSpan index00,
        TimeSpan index01,
        string songNameInCueFile,
		DiscNumber? discNumber = null)
    {
        var songCreationResult = Create(discId, title, order, discNumber);

        if (songCreationResult.IsFailure)
        {
            return songCreationResult;
        }

        var song = songCreationResult.Value;
        var settingPlayInfoResult = song.SetPlaybackInfo(executableFilePath, duration, cueFilePath, index00, index01, songNameInCueFile);

        if (settingPlayInfoResult.IsFailure)
        {
            return Result.Failure<Song>(settingPlayInfoResult.Error);
        }

        return song;
    }

    public Result SetPlaybackInfo(string executableFilePath, TimeSpan duration)
    {
        var creatingPlaybackInfoResult = PlaybackInfo.Create(executableFilePath, Id, duration);
        if (creatingPlaybackInfoResult.IsFailure)
        {
            return Result.Failure(creatingPlaybackInfoResult.Error);
        }

        PlaybackInfo = creatingPlaybackInfoResult.Value;
        return Result.Success();
    }

    public Result SetPlaybackInfo(
        string executableFilePath, 
        TimeSpan duration, 
        string cueFilePath,
        TimeSpan index00,
        TimeSpan index01,
        string songNameInCueFile)
    {
        var creatingPlaybackInfoResult = PlaybackInfo.Create(executableFilePath, Id, duration, cueFilePath, index00, index01, songNameInCueFile);
        if (creatingPlaybackInfoResult.IsFailure)
        {
            return Result.Failure(creatingPlaybackInfoResult.Error);
        }

        PlaybackInfo = creatingPlaybackInfoResult.Value;
        return Result.Success();
    }

    public Result SetDiscNumber(DiscNumber discNumber)
    {
        if (discNumber is null)
        {
            return Result.Failure(DomainErrors.NullPassed(nameof(discNumber)));
        }

        DiscNumber = discNumber;
        return Result.Success();
    }

    public Result SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure(DomainErrors.NullOrEmptyStringPassed("song title"));
        }

        Title = title;
        return Result.Success();
    }

    public Result SetOrder(int order)
    {
        if (int.IsNegative(order))
        {
            return Result.Failure<Song>(DomainErrors.Song.SongOrderCouldNotBeNegativeNumber);
        }

        Order = order;
        return Result.Success();
    }

    #endregion
}

public record SongId(Guid Value)
{
    public static SongId Create() => new(Guid.NewGuid());
}
