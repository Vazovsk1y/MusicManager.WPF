﻿using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Dtos;

namespace MusicManager.Services
{
    public interface ISongwriterService
    {
        Task<Result> SaveFromFolderAsync(SongwriterFolder songwriterFolder, CancellationToken cancellationToken = default);

        Task<Result<IEnumerable<SongwriterDTO>>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}