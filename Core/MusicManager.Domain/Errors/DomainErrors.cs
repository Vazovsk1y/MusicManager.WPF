using MusicManager.Domain.Shared;
using System.Text;

namespace MusicManager.Domain.Errors;

internal static class DomainErrors
{
    #region --Particular domain classes errors--

    internal static class ProductionInfo
    {
        public static Error IncorrectYearPassed(string year) => new($"The passed [{year}] year wasn't correct.");
    } 

    internal static class EntityDirectoryInfo
    {
        public static Error InvalidPathPassed(string path) => new($"The passed path [{path}] wasn't a directory path.");
    }

    internal static class PlaybackInfo
    {
        public static Error IncorrectCuePathPassed(string cuePath) => new($"Passed cue file path [{cuePath}] isn't correct.");

        public static readonly Error CueFileNotPlacedInTheExecutableFileFolder = new("Cue file must be place in the same folder like executable file.");
    }

    internal static class Disc
    {
        public static Error CoverWithPassedPathAlreadyAdded(string coverPath) => new ($"Cover with passed path [{coverPath}] has already added.");

        public static readonly Error UnableToRemoveSongWithPassedId = new("The song with the passed id has not been added deletion is not possible.");

		public static readonly Error ProductionYearCanNotBeNullForOtherTypesExceptBootlegAndUnknown = new("Production year must be set for other disc types except bootleg and unknown.");
	}

    internal static class Song
    {
        public static readonly Error SongOrderCouldNotBeNegativeNumber = new("Song order must be non-negative number.");
	}

    internal static class Songwriter
    {
		public static readonly Error UnableToRemoveCompilationWithPassedId = new("The compilation with the passed id has not been added deletion is not possible.");

		public static readonly Error UnableToRemoveMovieWithPassedId = new("The movie with the passed id has not been added deletion is not possible.");
	}

    internal static class DiscNumber
    {
        public static readonly Error DiscNumberDigitMustBeGreaterThanZero = new("Disc number must be greater than 0.");
    }

    #endregion

    #region --Base domain errors--

    public static Error NullOrEmptyStringPassed(params string[] valuesNames)
    {
        var builder = new StringBuilder("The passed row/s ");

        foreach (var valueName in valuesNames)
        {
            builder.Append($"{valueName}, ");
        }

        builder.Append(valuesNames.Length > 1 ? "arguments were " : "argument was ");

        return new Error(builder.Append("equal to null.").ToString());
    }

    public static Error NullOrEmptyStringPassed() => new($"Some of the string arguments passed were equal to null or was empty.");

    public static Error NullPassed(string entityName) => new($"Passed {entityName} was equal to null.");

    public static Error PassedEntityAlreadyAdded(string entityName) => new($"Passed {entityName} has already added.");

    #endregion
}
