using MusicManager.Domain.Common;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using System.Text.RegularExpressions;

namespace MusicManager.Domain.Services.Implementations
{
    public partial class FileToSongService : IPathToSongService
    {
        #region --Fields--

        private readonly ICueFileInteractor _cueFileInteractor;

        [GeneratedRegex("^\\d+")]
        private static partial Regex GetSongNumberFromRow();

        [GeneratedRegex(@"^CD(\d+)")]
        private static partial Regex IsDiscNumber();

        #endregion

        #region --Properties--



        #endregion

        #region --Constructors--

        public FileToSongService(ICueFileInteractor cueFileInteractor)
        {
            _cueFileInteractor = cueFileInteractor;
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
            var fileName = Path.GetFileName(songInfo.Name);

            var parentDirectoryName = new FileInfo(songInfo.Name).Directory?.Name ?? string.Empty;
            var discNumberMatch = IsDiscNumber().Match(parentDirectoryName);

            if (discNumberMatch.Success)
            {
                return Task.FromResult(
                    Song.Create(
                    parentId,
                    songInfo.Tag.Title ?? Path.GetFileNameWithoutExtension(fileName),
                    songInfo.Tag.Track > 0 ? (int)songInfo.Tag.Track : GetSongNumberFromFileName(fileName),
                    discNumberMatch.Value,
                    songFilePath,
                    songInfo.Properties.Duration
                    ));
            }

            var songCreationResult = Song.Create(
                parentId,
                songInfo.Tag.Title ?? Path.GetFileNameWithoutExtension(fileName),
                songInfo.Tag.Track > 0 ? (int)songInfo.Tag.Track : GetSongNumberFromFileName(fileName),
                songFilePath,
                songInfo.Properties.Duration
                );

            if (songCreationResult.IsFailure)
            {
                return Task.FromResult(Result.Failure<Song>(songCreationResult.Error));
            }

            return Task.FromResult(Result.Success(songCreationResult.Value));
        }

        public async Task<Result<IEnumerable<Song>>> GetEntitiesFromCueFileAsync(string songFilePath, string cueFilePath, DiscId parentId)
        {
            var songInfoResult = GetSongFileInfo(songFilePath);
            if (songInfoResult.IsFailure)
            {
                return Result.Failure<IEnumerable<Song>>(songInfoResult.Error);
            }

            using var songFileInfo = songInfoResult.Value;
            TimeSpan allSongFileDuration = songFileInfo.Properties.Duration;
            var fileName = Path.GetFileName(songFileInfo.Name);

            var parentDirectoryName = new FileInfo(songFileInfo.Name).Directory?.Name ?? string.Empty;
            var discNumberMatch = IsDiscNumber().Match(parentDirectoryName);
            var cueFileTracksGettingResult = await _cueFileInteractor.GetTracksAsync(cueFilePath);

            if (cueFileTracksGettingResult.IsFailure)
            {
                return Result.Failure<IEnumerable<Song>>(cueFileTracksGettingResult.Error);
            }

            var cueFileTracks = cueFileTracksGettingResult.Value.ToList();
            if (discNumberMatch.Success)
            {
                return ParseCueFileTracksToSongs(
                    cueFileTracks, 
                    parentId, 
                    songFilePath, 
                    cueFilePath, 
                    allSongFileDuration,
                    discNumberMatch.Value);
            }

            return ParseCueFileTracksToSongs(
                cueFileTracks, 
                parentId, 
                songFilePath, 
                cueFilePath, 
                allSongFileDuration);
        }

        #endregion

        #region __Private__

        private Result<TagLib.File> GetSongFileInfo(string songPath)
        {
            TagLib.File songInfo;
            try
            {
                songInfo = TagLib.File.Create(songPath);
            }
            catch (Exception e)
            {
                return Result.Failure<TagLib.File>(new Error(e.GetType().Name + '\n' + e.Message));
            }
            return songInfo;
        }

        private Result<IEnumerable<Song>> ParseCueFileTracksToSongs(
            IEnumerable<ICueFileTrack> cueFileTracks,
            DiscId parent,
            string songFilePath,
            string cueFilePath,
            TimeSpan allSongFileDuration,
            string? discNumber = null)
        {
            var results = new List<Song>();
            var tracks = cueFileTracks.ToList();

            for (int i = 1; i < tracks.Count; i++)
            {
                var previousTrack = tracks[i - 1];
                var currentTrack = tracks[i];

                var songCreationResult = discNumber is null ?
                    Song.Create(
                    parent,
                    previousTrack.Title,
                    previousTrack.TrackPosition,
                    songFilePath,
                    currentTrack.Index01 - previousTrack.Index01,
                    cueFilePath,
                    previousTrack.Index00,
                    previousTrack.Index01)
                    :
                    Song.Create(
                    parent, 
                    previousTrack.Title,
                    previousTrack.TrackPosition,
                    discNumber,
                    songFilePath,
                    currentTrack.Index01 - previousTrack.Index01,
                    cueFilePath,
                    previousTrack.Index00,
                    previousTrack.Index01
                    );

                if (songCreationResult.IsFailure)
                {
                    return Result.Failure<IEnumerable<Song>>(songCreationResult.Error);
                }

                results.Add(songCreationResult.Value);
            }

            var lastTrack = tracks.Last();
            var lastSongCreationResult = discNumber is null ?
                    Song.Create(
                    parent,
                    lastTrack.Title,
                    lastTrack.TrackPosition,
                    songFilePath,
                    allSongFileDuration - lastTrack.Index01,
                    cueFilePath,
                    lastTrack.Index00,
                    lastTrack.Index01)
                    :
                    Song.Create(
                    parent,
                    lastTrack.Title,
                    lastTrack.TrackPosition,
                    discNumber,
                    songFilePath,
                    allSongFileDuration - lastTrack.Index01,
                    cueFilePath,
                    lastTrack.Index00,
                    lastTrack.Index01
                    );

            if (lastSongCreationResult.IsSuccess)
            {
                results.Add(lastSongCreationResult.Value);
                return results;
            }

            return Result.Failure<IEnumerable<Song>>(lastSongCreationResult.Error);
        }

        private int GetSongNumberFromFileName(string fileName)
        {
            var match = GetSongNumberFromRow().Match(fileName);
            if (match.Success)
            {
                _ = int.TryParse(match.Value, out int result);
                return result;
            }

            return 0;
        }

        #endregion

        #endregion
    }
}
