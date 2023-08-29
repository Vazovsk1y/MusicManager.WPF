using Microsoft.Extensions.DependencyInjection;

namespace MusicManager.Domain.Services.Implementations
{
    public static class Registrator
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services) => services
            .AddTransient<ICueFileInteractor, CueFileInteractor>()

            // Folder or file to entities converting services.
            .AddScoped<IFolderToSongwriterService, FolderToSongwriterService>()
            .AddScoped<IFolderToMovieService, FolderToMovieService>()
            .AddScoped<IFolderToCompilationService, FolderToCompilationService>()
            .AddScoped<IFolderToMovieReleaseService, FolderToMovieReleaseService>()
            .AddScoped<IFileToSongService, FileToSongService>()

            // Entities to folder or file converting services.
            .AddScoped<ISongwriterToFolderService, SongwriterToFolderService>()
            .AddScoped<IMovieToFolderService, MovieToFolderService>()
            .AddScoped<ICompilationToFolderService, CompilationToFolderService>()
            .AddScoped<IMovieReleaseToFolderService, MovieReleaseToFolderService>()
            .AddScoped<ISongToFileService, SongToFileService>()
            ;
    }
}
