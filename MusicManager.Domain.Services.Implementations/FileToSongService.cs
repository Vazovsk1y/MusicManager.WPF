using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using System.Text.RegularExpressions;

namespace MusicManager.Domain.Services.Implementations
{
    public partial class FileToSongService : IPathToSongService
    {
        #region --Fields--

        private readonly ICueFileInteractor _cueFileInteractor;

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

            var songInfo = songInfoResult.Value;
            var fileName = Path.GetFileName(songInfo.Name);
            var discNumberMatch = FindDiscNumber().Match(fileName);

            if (discNumberMatch.Success)
            {
                return Task.FromResult(
                    Song.Create(
                    parentId,
                    songInfo.Tag.Title ?? Path.GetFileNameWithoutExtension(fileName),
                    TrimDiscNumber(discNumberMatch.Groups[0].Value),
                    songFilePath,
                    songInfo.Properties.Duration
                    ));
            }

            var songCreationResult = Song.Create(
                parentId,
                songInfo.Tag.Title ?? Path.GetFileNameWithoutExtension(fileName),
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

            var songFileInfo = songInfoResult.Value;
            TimeSpan allSongFileDuration = songFileInfo.Properties.Duration;
            var fileName = Path.GetFileName(songFileInfo.Name);
            var discNumberMatch = FindDiscNumber().Match(fileName);

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
                    TrimDiscNumber(discNumberMatch.Groups[0].Value));
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

        [GeneratedRegex(@"\((CD\d)\)")]
        private static partial Regex FindDiscNumber();

        private Result<TagLib.File> GetSongFileInfo(string songPath)
        {
            TagLib.File songInfo;
            try
            {
                songInfo = TagLib.File.Create(songPath);
            }
            catch (Exception e)
            {
                return Result.Failure<TagLib.File>(new Error(e.Message));
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
                    songFilePath,
                    currentTrack.Index01 - previousTrack.Index01,
                    cueFilePath)
                    :
                    Song.Create(
                    parent, 
                    previousTrack.Title,
                    discNumber,
                    songFilePath,
                    currentTrack.Index01 - previousTrack.Index01,
                    cueFilePath
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
                    songFilePath,
                    allSongFileDuration - lastTrack.Index01,
                    cueFilePath)
                    :
                    Song.Create(
                    parent,
                    lastTrack.Title,
                    discNumber,
                    songFilePath,
                    allSongFileDuration - lastTrack.Index01,
                    cueFilePath
                    );

            if (lastSongCreationResult.IsSuccess)
            {
                results.Add(lastSongCreationResult.Value);
                return results;
            }

            return Result.Failure<IEnumerable<Song>>(lastSongCreationResult.Error);
        }

        private string TrimDiscNumber(string untrimedDiscNumberRow) => untrimedDiscNumberRow
            .Replace("(", string.Empty)
            .Replace(")", string.Empty);

        #endregion

        #endregion
    }
}
