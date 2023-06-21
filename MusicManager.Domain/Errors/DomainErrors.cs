using MusicManager.Domain.Shared;
using System.Text;

namespace MusicManager.Domain.Errors;

internal static class DomainErrors
{
    #region --Particular domain classes errors--

    internal static class ProductInfoErrors
    {
        public static readonly Error IncorrectYearPassed = new("The passed year wasn't correct.");
    }

    internal static class EntityDirectoryInfoErrors
    {
        public static readonly Error IncorrectPathPassed = new("The passed path don't ends by passed file name.");
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
