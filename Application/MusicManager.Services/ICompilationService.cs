﻿using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts.Base;
using MusicManager.Services.Contracts.Dtos;

namespace MusicManager.Services
{
    public interface ICompilationService
    {
        Task<Result> SaveFromFolderAsync(DiscFolder compilationFolder, SongwriterId songwriterId, CancellationToken cancellationToken = default);

        Task<Result<IEnumerable<CompilationDTO>>> GetAllAsync(SongwriterId songwriterId, CancellationToken cancellation = default);
    }
}