using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Entities;
using MusicManager.Domain.Shared;
using MusicManager.Repositories.Data;
using MusicManager.Services.Contracts.Dtos;

namespace MusicManager.Services.Implementations;

public class DirectorService : IDirectorService
{
    private readonly IApplicationDbContext _context;

    public DirectorService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IReadOnlyCollection<DirectorDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context
            .Directors
            .Select(e => new DirectorDTO(e.Id, e.FullName))
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<DirectorId>> SaveAsync(string fullName, CancellationToken cancellationToken = default)
    {
        var creationResult = Director.Create(fullName);

        if (creationResult.IsFailure)
        {
            return Result.Failure<DirectorId>(creationResult.Error);
        }

        await _context.Directors.AddAsync(creationResult.Value, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(creationResult.Value.Id);
    }
}








