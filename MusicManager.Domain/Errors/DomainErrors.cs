using MusicManager.Domain.Shared;
using System.Text;

namespace MusicManager.Domain.Errors;

internal static class DomainErrors
{
    internal static class ProductInfoErrors
    {
        public static readonly Error IncorrectYearPassed = new("The passed year wasn't correct.");
    }

    public static Error NullPassedError(params string[] valuesNames)
    {
        var builder = new StringBuilder("The passed ");

        foreach (var valueName in valuesNames)
        {
            builder.Append($"{valueName}, ");
        }

        builder.Append(valuesNames.Length > 1 ? "were " : "was ");

        return new Error(builder.Append("equal to null.").ToString());
    }

    public static Error NullPassedError() => new($"Some of the arguments passed were equal to null.");
}
