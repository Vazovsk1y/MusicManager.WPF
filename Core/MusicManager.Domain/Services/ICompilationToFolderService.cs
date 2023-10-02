﻿using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services;

public interface ICompilationToFolderService
{
    Task<Result<string>> CreateAssociatedFolderAndFileAsync(Compilation compilation, Songwriter parent);

    Task<Result<string>> UpdateIfExistsAsync(Compilation compilation, CancellationToken cancellationToken = default);
}