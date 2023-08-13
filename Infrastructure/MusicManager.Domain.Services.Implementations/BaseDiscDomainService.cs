using MusicManager.Domain.Enums;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Shared;
using System.Text.RegularExpressions;

namespace MusicManager.Domain.Services.Implementations;

public abstract partial class BaseDiscDomainService : BaseDomainService
{
    protected Result<(DiscType type, string identificator, string? prodCountry, int? prodYear)> GetDiscComponentsFromDirectoryName(string discDirectoryName)
    {
        var match = FindAllDiscComponents().Match(discDirectoryName);
        if (match.Success)
        {
            var discTypeCreationResult = match.Groups[1].Value.CreateDiscType();
            if (discTypeCreationResult.IsFailure)
            {
                return Result.Failure<(DiscType, string, string?, int?)>(discTypeCreationResult.Error);
            }

            _ = int.TryParse(match.Groups[4].Value, out int year);
            return (discTypeCreationResult.Value, match.Groups[2].Value, match.Groups[3].Value, year == 0 ? null : year);
        }

        return Result.Failure<(DiscType, string, string?, int?)>(new Error($"Unable to get some of the required components from disc directory name [{discDirectoryName}]."));
    }

    [GeneratedRegex(@"^(\S+)\s+(.*?)(?:\s+-\s+(\S+)(?:\s+-\s+(\d{4}))?.*?)?$")]
    private partial Regex FindAllDiscComponents();
}
