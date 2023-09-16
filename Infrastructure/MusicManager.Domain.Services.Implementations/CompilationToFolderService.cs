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
    private readonly IRoot _root;

    public CompilationToFolderService(
        IApplicationDbContext dbContext, 
        IRoot root)
    {
        _dbContext = dbContext;
        _root = root;
    }

    public async Task<Result<string>> CreateAssociatedFolderAndFileAsync(Compilation compilation, Songwriter parent)
    {
        if (parent.EntityDirectoryInfo is null)
        {
            return Result.Failure<string>(new Error("Parent directory info is not created."));
        }

        var rootDirectory = new DirectoryInfo(Path.Combine(_root.CombineWith(parent.EntityDirectoryInfo.Path), DomainServicesConstants.COMPILATIONS_FOLDER_NAME));
        if (!rootDirectory.Exists)
        {
            return Result.Failure<string>(new Error("Parent directory is not exists."));
        }

        string createdCompilationDirectoryName = GetDirectoryName(compilation);
        string createdCompilationDirectoryFullPath = Path.Combine(rootDirectory.FullName, createdCompilationDirectoryName);
        string createdCompilationRelationalPath = createdCompilationDirectoryFullPath.GetRelational(_root);

        if (Directory.Exists(createdCompilationDirectoryFullPath)
            || await _dbContext.Compilations.AnyAsync(e => e.EntityDirectoryInfo == EntityDirectoryInfo.Create(createdCompilationRelationalPath).Value))
        {
            return Result.Failure<string>(new Error("Directory for this compilation is already exists or compilation with that directory info is already added to database."));
        }

        var createdCompilationDirectoryInfo = DirectoryHelper.CreateIfNotExists(createdCompilationDirectoryFullPath);
        createdCompilationDirectoryInfo.CreateSubdirectory(DomainServicesConstants.COVERS_FOLDER_NAME);

        string jsonFileInfoPath = Path.Combine(createdCompilationDirectoryFullPath, CompilationEntityJson.FileName);
        await compilation
            .ToJson()
            .AddSerializedJsonEntityToAsync(jsonFileInfoPath);

        return createdCompilationRelationalPath;
    }

    public async Task<Result<string>> UpdateIfExistsAsync(Compilation compilation, CancellationToken cancellationToken = default)
    {
        if (compilation.EntityDirectoryInfo is null)
        {
            return Result.Failure<string>(new Error($"Associated folder isn't created."));
        }

        var currentDirectory = new DirectoryInfo(_root.CombineWith(compilation.EntityDirectoryInfo.Path));
        if (!currentDirectory.Exists)
        {
            return Result.Failure<string>(new Error($"Associated folder isn't exists."));
        }

        string newDirectoryName = GetDirectoryName(compilation);
        string newDirecotryFullPath = Path.Combine(Path.GetDirectoryName(currentDirectory.FullName), newDirectoryName);

        if (currentDirectory.FullName != newDirecotryFullPath)
        {
            currentDirectory.MoveTo(newDirecotryFullPath);
        }

        await compilation
           .ToJson()
           .AddSerializedJsonEntityToAsync(Path.Combine(currentDirectory.FullName, CompilationEntityJson.FileName));

        return Result.Success(newDirecotryFullPath.GetRelational(_root));
    }

    private string GetDirectoryName(Compilation compilation)
    {
        string baseCompilationDirectoryName = $"{compilation.Type.Value} {compilation.Identifier}";
        if (compilation.Type == DiscType.Bootleg)
        {
            return baseCompilationDirectoryName;
        }

        string createdCompilationDirectoryName = $"{baseCompilationDirectoryName} {DomainServicesConstants.DiscDirectoryNameSeparator} {compilation.ProductionInfo.Country ?? ProductionInfo.UndefinedCountry} " +
        $"{DomainServicesConstants.DiscDirectoryNameSeparator} {compilation.ProductionInfo.Year!}";

        return createdCompilationDirectoryName;
    }
}
