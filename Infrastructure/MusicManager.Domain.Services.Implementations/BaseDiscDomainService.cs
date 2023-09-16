﻿using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace MusicManager.Domain.Services.Implementations;

public abstract partial class BaseDiscDomainService : BaseDomainService
{
    protected BaseDiscDomainService(IRoot userConfig) : base(userConfig)
    {
    }

    protected Result<(DiscType type, string identificator, string? prodCountry, int prodYear)> GetDiscComponentsFromDirectoryName(string discDirectoryName)
    {
        var match = FindAllDiscComponents().Match(discDirectoryName);
        if (match.Success)
        {
            var discTypeCreationResult = DiscType.Create(match.Groups[1].Value);
            if (discTypeCreationResult.IsFailure)
            {
                return Result.Failure<(DiscType, string, string?, int)>(discTypeCreationResult.Error);
            }

            _ = int.TryParse(match.Groups[4].Value, out int year);
            return (discTypeCreationResult.Value, match.Groups[2].Value, match.Groups[3].Value, year);
        }

        return Result.Failure<(DiscType, string, string?, int)>(new Error($"Unable to get some of the required components from disc directory name [{discDirectoryName}]."));
    }

    [GeneratedRegex(@"^(\S+)\s+(.*?)(?:\s+-\s+([^-\n]+)(?:\s+-\s+(\d{4})).*?)?$")]
    private partial Regex FindAllDiscComponents();
}
