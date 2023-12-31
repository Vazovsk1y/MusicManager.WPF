﻿using Microsoft.EntityFrameworkCore;
using MusicManager.Domain.Extensions;
using MusicManager.Domain.Models;
using MusicManager.Domain.Services.Implementations.Errors;
using MusicManager.Domain.Services.Storage;
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

    public async Task<Result<string>> CreateAssociatedFolderAndFileAsync(Compilation compilation, Songwriter parent, CancellationToken cancellationToken = default)
    {
        if (parent.AssociatedFolderInfo is null)
        {
            return Result.Failure<string>(DomainServicesErrors.ParentAssociatedDirectoryIsNotCreated);
        }

        var rootDirectory = new DirectoryInfo(Path.Combine(_root.CombineWith(parent.AssociatedFolderInfo.Path), DomainServicesConstants.COMPILATIONS_FOLDER_NAME));
        if (!rootDirectory.Exists)
        {
            return Result.Failure<string>(DomainServicesErrors.PassedDirectoryIsNotExists(rootDirectory.FullName));
        }

        string createdCompilationDirectoryName = GetDirectoryName(compilation);
        string createdCompilationDirectoryFullPath = Path.Combine(rootDirectory.FullName, createdCompilationDirectoryName);
        string createdCompilationRelationalPath = createdCompilationDirectoryFullPath.GetRelational(_root);

        if (Directory.Exists(createdCompilationDirectoryFullPath)
            || await _dbContext.Compilations.AnyAsync(e => e.AssociatedFolderInfo == EntityDirectoryInfo.Create(createdCompilationRelationalPath).Value, cancellationToken))
        {
            return Result.Failure<string>(DomainServicesErrors.DirectoryForEntityIsAlreadyCreated("compilation", createdCompilationDirectoryFullPath));
        }

        var createdCompilationDirectoryInfo = DirectoryHelper.CreateIfNotExists(createdCompilationDirectoryFullPath);
        createdCompilationDirectoryInfo.CreateSubdirectory(DomainServicesConstants.COVERS_FOLDER_NAME);

        string jsonFileInfoPath = Path.Combine(createdCompilationDirectoryFullPath, CompilationEntityJson.FileName);
        await compilation
            .ToJson()
            .AddSerializedJsonEntityToAsync(jsonFileInfoPath);

        return createdCompilationRelationalPath;
    }

    public async Task<Result<string>> UpdateAsync(Compilation compilation, CancellationToken cancellationToken = default)
    {
        if (compilation.AssociatedFolderInfo is null)
        {
            return Result.Failure<string>(DomainServicesErrors.AssociatedFolderIsNotCreated);
        }

        var currentDirectory = new DirectoryInfo(_root.CombineWith(compilation.AssociatedFolderInfo.Path));
        if (!currentDirectory.Exists)
        {
            return Result.Failure<string>(DomainServicesErrors.PassedDirectoryIsNotExists(currentDirectory.FullName));
        }

        string newDirectoryName = GetDirectoryName(compilation);
        string newDirecotryFullPath = Path.Combine(Path.GetDirectoryName(currentDirectory.FullName)!, newDirectoryName);

        if (currentDirectory.FullName != newDirecotryFullPath)
        {
            currentDirectory.MoveTo(newDirecotryFullPath);
        }

        await compilation
           .ToJson()
           .AddSerializedJsonEntityToAsync(Path.Combine(currentDirectory.FullName, CompilationEntityJson.FileName));

        return Result.Success(newDirecotryFullPath.GetRelational(_root));
    }

    private static string GetDirectoryName(Compilation compilation)
    {
        string baseCompilationDirectoryName = $"{compilation.Type.Value} {compilation.Identifier}";
        if (compilation.Type == DiscType.Bootleg || compilation.Type == DiscType.Unknown)
        {
            return baseCompilationDirectoryName;
        }

        string createdCompilationDirectoryName = $"{baseCompilationDirectoryName} {DomainServicesConstants.DiscFolderNameSeparator} {compilation.ProductionInfo.Country ?? ProductionInfo.UndefinedCountry} " +
        $"{DomainServicesConstants.DiscFolderNameSeparator} {compilation.ProductionInfo.Year!}";

        return createdCompilationDirectoryName;
    }
}
