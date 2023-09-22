﻿namespace MusicManager.Services.Contracts.Base;

public record DiscFolder(string Path, IReadOnlyCollection<SongFile> Songs, IReadOnlyCollection<string> CoversPaths, string? LinkPath = null);
