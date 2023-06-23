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
        public static Error PassedPathDontEndsByPassedName(string path, string name) => new($"The passed [{path}] path don't ends by passed [{name}] file name.");
    }

    #endregion

    #region --Base domain errors--

    public static Error NullOrEmptyStringPassedError(params string[] valuesNames)
    {
        var builder = new StringBuilder("The passed string ");

        foreach (var valueName in valuesNames)
        {
            builder.Append($"{valueName}, ");
        }

        builder.Append(valuesNames.Length > 1 ? "arguments were " : "argument was ");

        return new Error(builder.Append("equal to null.").ToString());
    }

    public static Error NullOrEmptyStringPassedError() => new($"Some of the string arguments passed were equal to null or was empty.");

    #endregion
}
