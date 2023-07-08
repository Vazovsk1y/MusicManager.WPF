﻿using MusicManager.Domain.Common;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Entities;

public class Cover
{
    public DiscId DiscId { get; private set; }

    public string FullPath { get; private set; }

    private Cover() { }

    internal static Result<Cover> Create(DiscId parent, string fullPath)
    {
        if (string.IsNullOrWhiteSpace(fullPath))
        {
            return Result.Failure<Cover>(DomainErrors.NullOrEmptyStringPassed(nameof(fullPath)));
        }

        return new Cover() 
        {
            FullPath = fullPath,
            DiscId = parent
        };
    }
}