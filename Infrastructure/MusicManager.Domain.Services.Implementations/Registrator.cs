﻿using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace MusicManager.Domain.Services.Implementations
{
    public static class Registrator
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services) => services
            .AddTransient<ICueFileInteractor, CueFileInteractor>()

            // Folder or file to entities converting services.
            .AddTransient<IFolderToSongwriterService, FolderToSongwriterService>()
            .AddTransient<IFolderToMovieService, FolderToMovieService>()
            .AddSingleton<IFolderToCompilationService, FolderToCompilationService>()
            .AddSingleton<IFolderToMovieReleaseService, FolderToMovieReleaseService>()
            .AddSingleton<IFileToSongService, FileToSongService>()

            // Entities to folder or file converting services.
            .AddScoped<ISongwriterToFolderService, SongwriterToFolderService>()
            .AddScoped<IMovieToFolderService, MovieToFolderService>()
            .AddScoped<ICompilationToFolderService, CompilationToFolderService>()
            .AddScoped<IMovieReleaseToFolderService, MovieReleaseToFolderService>()
            .AddScoped<ISongToFileService, SongToFileService>()
            ;
    }
}
