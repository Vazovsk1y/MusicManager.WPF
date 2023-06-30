using MusicManager.Domain.Helpers;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Shared;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MusicManager.Domain.Services.Implementations
{
    

    public partial class FileToSongService : IPathToSongService
    {
        [GeneratedRegex(@"\((CD\d)\)")]
        private static partial Regex FindDiscNumberRegex();

        private const string CDKeyWord = "CD";

        private readonly ICueFileInteractor _cueFileInteractor;

        public FileToSongService(ICueFileInteractor cueFileInteractor)
        {
            _cueFileInteractor = cueFileInteractor;
        }

        public async Task<Result<IEnumerable<Song>>> GetEntitiesFromCueFileAsync(string songFilePath, string cueFilePath, DiscId parentId)
        {
            if (!PathValidator.IsValid(songFilePath))
            {
                return Result.Failure<IEnumerable<Song>>(DomainServicesErrors.PassedDirectoryPathIsInvalid(songFilePath));
            }

            var fileInfo = new FileInfo(songFilePath);
            if (!fileInfo.Exists)
            {
                return Result.Failure<IEnumerable<Song>>(DomainServicesErrors.PassedDirectoryIsNotExists(songFilePath));
            }

            var songs = new List<Song>();   
            if (fileInfo.Name.Contains(CDKeyWord))
            {
                var cdMatch = FindDiscNumberRegex().Match(fileInfo.Name);
                if (!cdMatch.Success)
                {
                    return Result.Failure<IEnumerable<Song>>(new Error("Can't get a disc number from file name."));
                }

                var tracksGettingResult1 = await _cueFileInteractor.GetTracksAsync(cueFilePath);

                if (tracksGettingResult1.IsFailure)
                {
                    return Result.Failure<IEnumerable<Song>>(tracksGettingResult1.Error);
                }

                var tracks1 = tracksGettingResult1.Value.ToList();
                for (int i = 1; i < tracks1.Count; i++)
                {
                    var previousTrack = tracks1[i-1];
                    var currentTrack = tracks1[i];

                    var songCreationResult = Song.Create(
                        previousTrack.Title, 
                        parentId, 
                        cdMatch.Groups[0].Value.Replace("(", "").Replace(")", ""));

                    if (songCreationResult.IsFailure)
                    {
                        return Result.Failure<IEnumerable<Song>>(songCreationResult.Error);
                    }

                    var song = songCreationResult.Value;
                    var settingSongFileInfoResult = song.SetSongFileInfo(
                        songFilePath, 
                        currentTrack.Index01 - previousTrack.Index00,
                        cueFilePath);

                    if (settingSongFileInfoResult.IsFailure)
                    {
                        return Result.Failure<IEnumerable<Song>>(settingSongFileInfoResult.Error);
                    }

                    songs.Add(song);
                }

                return songs;
            }

            var tracksGettingResult = await _cueFileInteractor.GetTracksAsync(cueFilePath);

            if (tracksGettingResult.IsFailure)
            {
                return Result.Failure<IEnumerable<Song>>(tracksGettingResult.Error);
            }

            var tracks = tracksGettingResult.Value.ToList();
            for (int i = 1; i < tracks.Count; i++)
            {
                var previousTrack = tracks[i - 1];
                var currentTrack = tracks[i];

                var songCreationResult = Song.Create(
                    previousTrack.Title,
                    parentId);

                if (songCreationResult.IsFailure)
                {
                    return Result.Failure<IEnumerable<Song>>(songCreationResult.Error);
                }

                var song = songCreationResult.Value;
                var settingSongFileInfoResult = song.SetSongFileInfo(
                    songFilePath,
                    currentTrack.Index01 - previousTrack.Index01,
                    cueFilePath);

                if (settingSongFileInfoResult.IsFailure)
                {
                    return Result.Failure<IEnumerable<Song>>(settingSongFileInfoResult.Error);
                }

                songs.Add(song);
            }

            return songs;
        }

        public Task<Result<Song>> GetEntityAsync(string songFilePath, DiscId parentId)
        {
            TagLib.File songInfo;
            try
            {
                songInfo = TagLib.File.Create(songFilePath);
            }
            catch (Exception e)
            {
                return Task.FromResult(Result.Failure<Song>(new Error(e.Message)));
            }

            if (Path.GetFileName(songInfo.Name).Contains(CDKeyWord))
            {
                var cdMatch = FindDiscNumberRegex().Match(songInfo.Name);
                if (!cdMatch.Success)
                {
                    return Task.FromResult(Result.Failure<Song>(new Error("Can't get a disc number from file name.")));
                }

                var songCreationResult = Song.Create(
                songInfo.Tag.Title,
                parentId,
                cdMatch.Groups[1].Value.Replace("(", "").Replace(")", "")
                );

                if (songCreationResult.IsFailure)
                {
                    return Task.FromResult(Result.Failure<Song>(songCreationResult.Error));
                }
            }

            var songCreationResultWithoutDiscNumber = Song.Create(
                songInfo.Tag.Title ?? songInfo.Name,
                parentId
                );

            if (songCreationResultWithoutDiscNumber.IsFailure)
            {
                return Task.FromResult(Result.Failure<Song>(songCreationResultWithoutDiscNumber.Error));
            }

            var song = songCreationResultWithoutDiscNumber.Value;
            var settingSongFileInfoResult = song.SetSongFileInfo(songFilePath, songInfo.Properties.Duration);

            if (settingSongFileInfoResult.IsFailure)
            {
                return Task.FromResult(Result.Failure<Song>(settingSongFileInfoResult.Error));
            }

            return Task.FromResult(Result.Success(song));
        }
    }
}
