﻿using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts.Base;

namespace MusicManager.Services
{
    public interface ICompilationService
    {
        Task<Result> SaveFromFolderAsync(IDiscFolder compilationFolder, SongwriterId songwriterId, CancellationToken cancellationToken = default);
    }
}