﻿using MusicManager.Services.Contracts.Base;

namespace MusicManager.Services.Contracts;

public record MovieFolder(
	string Path, 
	IEnumerable<DiscFolder> MoviesReleasesFolders);
