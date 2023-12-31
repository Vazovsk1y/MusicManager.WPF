﻿using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts.Base;

namespace MusicManager.Services.Contracts.Factories;

public interface IDiscFolderFactory
{
    Result<DiscFolder> Create(DirectoryInfo discDirectoryInfo, string? linkPath = null);
}


