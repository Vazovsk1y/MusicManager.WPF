using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;
using MusicManager.Repositories.Data;
using MusicManager.Utils;

namespace MusicManager.Domain.Services.Implementations;

public class CompilationToFolderService : ICompilationToFolderService
{
    private readonly IApplicationDbContext _dbContext;

    public CompilationToFolderService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<string>> CreateAssociatedFolderAndFileAsync(Compilation compilation, Songwriter parent)
    {
        if (parent.EntityDirectoryInfo is null)
        {
            return Result.Failure<string>(new Error("Parent directory info is not created."));
        }

        var rootPath = Path.Combine(parent.EntityDirectoryInfo.FullPath, DomainServicesConstants.COMPILATIONS_FOLDER_NAME);
        if (!Directory.Exists(rootPath))
        {
            return Result.Failure<string>(new Error("Parent directory is not exists."));
        }

        string baseCompilationDirectoryName = $"{compilation.Type.MapToString()} {compilation.Identifier}";
        string createdCompilationDirectoryName = compilation.ProductionInfo is null ?
        baseCompilationDirectoryName
        :
        $"{baseCompilationDirectoryName} {DomainServicesConstants.DiscDirectoryNameSeparator} {compilation.ProductionInfo.Country} " +
        $"{DomainServicesConstants.DiscDirectoryNameSeparator} {compilation.ProductionInfo.Year}";

        string createdCompilationDirectoryFullPath = Path.Combine(rootPath, createdCompilationDirectoryName);
        if (Directory.Exists(createdCompilationDirectoryFullPath)
            || await _dbContext.Compilations.AnyAsync(e => e.EntityDirectoryInfo == EntityDirectoryInfo.Create(createdCompilationDirectoryFullPath).Value))
        {
            return Result.Failure<string>(new Error("Directory for this compilation is already exists or compilation with that directory info is already added to database."));
        }

        var createdCompilationDirectoryInfo = DirectoryHelper.CreateIfNotExists(createdCompilationDirectoryFullPath);
        createdCompilationDirectoryInfo.CreateSubdirectory(DomainServicesConstants.COVERS_FOLDER_NAME);

        string jsonFileInfoPath = Path.Combine(createdCompilationDirectoryFullPath, CompilationEntityJson.FileName);
        await compilation
            .ToJson()
            .AddSerializedJsonEntityToAsync(jsonFileInfoPath);

        return createdCompilationDirectoryFullPath;
    }
}
