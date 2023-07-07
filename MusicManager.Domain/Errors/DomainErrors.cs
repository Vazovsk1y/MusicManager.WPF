using MusicManager.Domain.Shared;
using System.Text;

namespace MusicManager.Domain.Errors;

internal static class DomainErrors
{
    #region --Particular domain classes errors--

    internal static class ProductInfo
    {
        public static Error IncorrectYearPassed(string year) => new($"The passed [{year}] year wasn't correct.");
    } 

    internal static class EntityDirectoryInfo
    {
        public static Error InvalidPathPassed(string path) => new($"The passed path [{path}] wasn't a directory path.");
    }

    internal static class SongPlayInfo
    {
        public static Error UndefinedExecutableTypePassed(string extension) => new($"Passed executable song file type [{extension}] is not support.");

        public static Error IncorrectCuePathPassed(string cuePath) => new($"Passed cue file path [{cuePath}] isn't correct.");
    }

    #endregion

    #region --Base domain errors--

    public static Error NullOrEmptyStringPassed(params string[] valuesNames)
    {
        var builder = new StringBuilder("The passed string ");

        foreach (var valueName in valuesNames)
        {
            builder.Append($"{valueName}, ");
        }

        builder.Append(valuesNames.Length > 1 ? "arguments were " : "argument was ");

        return new Error(builder.Append("equal to null.").ToString());
    }

    public static Error NullOrEmptyStringPassed() => new($"Some of the string arguments passed were equal to null or was empty.");

    public static Error NullEntityPassed(string entityName) => new($"Passed {entityName} was null.");

    public static Error EntityAlreadyExists(string entityName) => new($"Passed {entityName} is already exists.");

    #endregion
}
