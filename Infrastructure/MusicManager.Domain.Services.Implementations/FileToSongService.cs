using Microsoft.Extensions.Logging;
using MusicManager.Domain.Common;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;
using NAudio.Wave;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace MusicManager.Domain.Services.Implementations
{
    public partial class FileToSongService : IFileToSongService
    {
        #region --Fields--

        private readonly ICueFileInteractor _cueFileInteractor;

        private readonly ConcurrentDictionary<(string songFilePath, DiscId parentId), Song> _simpleFilesCache = new();

        private readonly ConcurrentDictionary<(string songFilePath, string cueFilePath, DiscId parentId), IReadOnlyCollection<Song>> _cueFilesCache = new();

        private readonly IRoot _root;

        private readonly ILogger<FileToSongService> _logger;

        #endregion

        #region --Properties--



        #endregion

        #region --Constructors--

        public FileToSongService(
            ICueFileInteractor cueFileInteractor,
            IRoot root,
            ILogger<FileToSongService> logger)
        {
            _cueFileInteractor = cueFileInteractor;
            _root = root;
            _logger = logger;
        }

        #endregion

        #region --Methods--

        #region __Public__

        public Task<Result<Song>> GetEntityAsync(string songFilePath, DiscId parentId)
        {
            var songInfoResult = GetSongFileInfo(songFilePath);
            if (songInfoResult.IsFailure)
            {
                return Task.FromResult(Result.Failure<Song>(songInfoResult.Error));
            }

            using var songInfo = songInfoResult.Value;
            if (_simpleFilesCache.TryGetValue((songFilePath, parentId), out var song))
            {
                return Task.FromResult(Result.Success(song));
            }

            var fileName = Path.GetFileName(songInfo.Name);
            string songName = songInfo.Tag.Title ?? Path.GetFileNameWithoutExtension(fileName);
            string songPath = songFilePath.GetRelational(_root);
            int songOrder = songInfo.Tag.Track > 0 ? (int)songInfo.Tag.Track : GetSongOrderFromFileName(fileName);
			var songDuration = GetDurationUsingNAudio(songFilePath) ?? songInfo.Properties.Duration;

			var parentDirectoryName = new FileInfo(songInfo.Name).Directory?.Name ?? string.Empty;
			var discNumberMatch = IsDiscNumber().Match(parentDirectoryName);
			if (discNumberMatch.Success)
            {
                var discNumberCreationResult = DiscNumber.Create(int.Parse(discNumberMatch.Groups[1].Value));
                if (discNumberCreationResult.IsFailure)
                {
                    return Task.FromResult(Result.Failure<Song>(discNumberCreationResult.Error));
                }

                var songCreationResult = Song.Create(
                    parentId,
                    songName,
                    songOrder,
                    songPath,
                    songDuration,
                    discNumberCreationResult.Value
                    );

                if (songCreationResult.IsFailure)
                {
                    return Task.FromResult(Result.Failure<Song>(songCreationResult.Error));
                }

                _simpleFilesCache[(songFilePath, parentId)] = songCreationResult.Value;
                return Task.FromResult(Result.Success(songCreationResult.Value));
                    
            }

            var songCreationResultWithoutDiscNumber = Song.Create(
                parentId,
                songName,
                songOrder,
                songPath,
                songDuration
                );

            if (songCreationResultWithoutDiscNumber.IsFailure)
            {
                return Task.FromResult(Result.Failure<Song>(songCreationResultWithoutDiscNumber.Error));
            }

            _simpleFilesCache[(songFilePath, parentId)] = songCreationResultWithoutDiscNumber.Value;
            return Task.FromResult(Result.Success(songCreationResultWithoutDiscNumber.Value));
        }

        public async Task<Result<IReadOnlyCollection<Song>>> GetEntitiesFromCueFileAsync(string cueFilePath, DiscId parentId)
        {
            var cueSheetResult = await _cueFileInteractor.GetCueSheetAsync(cueFilePath);
            if (cueSheetResult.IsFailure)
            {
                return Result.Failure<IReadOnlyCollection<Song>>(cueSheetResult.Error);
            }

            var songExecutableFilePath = Path.Combine(Path.GetDirectoryName(cueFilePath)!, cueSheetResult.Value.ExecutableFileName);
            var songInfoResult = GetSongFileInfo(songExecutableFilePath);
            if (songInfoResult.IsFailure)
            {
                return Result.Failure<IReadOnlyCollection<Song>>(songInfoResult.Error);
            }

            using var songFileInfo = songInfoResult.Value;
            if (_cueFilesCache.TryGetValue((songExecutableFilePath, cueFilePath,  parentId), out var songs))
            {
                return Result.Success(songs);
            }

            TimeSpan fullSongFileDuration = GetDurationUsingNAudio(songExecutableFilePath) ?? songFileInfo.Properties.Duration;
            var fileName = Path.GetFileName(songFileInfo.Name);

            var parentDirectoryName = new FileInfo(songFileInfo.Name).Directory?.Name ?? string.Empty;
            var discNumberMatch = IsDiscNumber().Match(parentDirectoryName);
            var cueFileTracks = cueSheetResult.Value.Tracks.ToList();

            if (discNumberMatch.Success)
            {
				var discNumberCreationResult = DiscNumber.Create(int.Parse(discNumberMatch.Groups[1].Value));
				if (discNumberCreationResult.IsFailure)
				{
					return Result.Failure<IReadOnlyCollection<Song>>(discNumberCreationResult.Error);
				}

				var result = ParseCueFileTracksToSongs(
                    cueFileTracks,
                    parentId,
                    songExecutableFilePath.GetRelational(_root),
                    cueFilePath.GetRelational(_root),
                    fullSongFileDuration,
                    discNumberCreationResult.Value);

                if (result.IsFailure)
                {
                    return Result.Failure<IReadOnlyCollection<Song>>(result.Error);
                }

                _cueFilesCache[(songExecutableFilePath, cueFilePath, parentId)] = result.Value;
                return result;
            }

            var resultWithoutDiscNumber = ParseCueFileTracksToSongs(
                    cueFileTracks,
                    parentId,
                    songExecutableFilePath.GetRelational(_root),
                    cueFilePath.GetRelational(_root),
                    fullSongFileDuration);

            if (resultWithoutDiscNumber.IsFailure)
            {
                return Result.Failure<IReadOnlyCollection<Song>>(resultWithoutDiscNumber.Error);
            }

            _cueFilesCache[(songExecutableFilePath, cueFilePath, parentId)] = resultWithoutDiscNumber.Value;
            return resultWithoutDiscNumber;
        }

        #endregion

        #region __Private__

        private Result<TagLib.File> GetSongFileInfo(string songPath)
        {
            TagLib.File? songInfo = null;
            try
            {
                songInfo = TagLib.File.Create(songPath);
                return songInfo;
            }
            catch (Exception e)
            {
                songInfo?.Dispose();
                _logger.LogError(e, "Something went wrong when trying to create songInfo.");
                return Result.Failure<TagLib.File>(new(e.Message));
            }
        }

        private TimeSpan? GetDurationUsingNAudio(string filePath)
        {
            try
            {
                using var reader = new AudioFileReader(filePath);
                return reader.TotalTime;
            }
            catch
            {
                return null;
            }
        }

        private Result<IReadOnlyCollection<Song>> ParseCueFileTracksToSongs(
            IEnumerable<CueFileTrack> cueFileTracks,
            DiscId parent,
            string songRelationalFilePath,
            string cueRelationalFilePath,
            TimeSpan allSongFileDuration,
            DiscNumber? discNumber = null)
        {
            var results = new List<Song>();
            var tracks = cueFileTracks.ToList();

            for (int i = 1; i < tracks.Count; i++)
            {
                var previousTrack = tracks[i - 1];
                var currentTrack = tracks[i];

                var songCreationResult =
                    Song.Create(
                    parent,
                    previousTrack.Title,
                    previousTrack.TrackOrder,
                    songRelationalFilePath,
                    currentTrack.Index01 - previousTrack.Index01,
                    cueRelationalFilePath,
                    previousTrack.Index00,
                    previousTrack.Index01,
                    previousTrack.Title,
                    discNumber);

                if (songCreationResult.IsFailure)
                {
                    return Result.Failure<IReadOnlyCollection<Song>>(songCreationResult.Error);
                }

                results.Add(songCreationResult.Value);
            }

            var lastTrack = tracks.Last();
            var lastSongCreationResult =
                    Song.Create(
                    parent,
                    lastTrack.Title,
                    lastTrack.TrackOrder,
                    songRelationalFilePath,
                    allSongFileDuration - lastTrack.Index01,
                    cueRelationalFilePath,
                    lastTrack.Index00,
                    lastTrack.Index01,
                    lastTrack.Title,
                    discNumber);
                   

            if (lastSongCreationResult.IsSuccess)
            {
                results.Add(lastSongCreationResult.Value);
                return results;
            }

            return Result.Failure<IReadOnlyCollection<Song>>(lastSongCreationResult.Error);
        }

		private int GetSongOrderFromFileName(string fileName)
		{
			var match = GetSongOrderFromRow().Match(fileName);
			_ = int.TryParse(match.Value, out int result);
            return result;
		}

		[GeneratedRegex("^\\d+")]
        private static partial Regex GetSongOrderFromRow();

        [GeneratedRegex(@"^CD(\d+)")]
        private static partial Regex IsDiscNumber();

        #endregion

        #endregion
    }
}
