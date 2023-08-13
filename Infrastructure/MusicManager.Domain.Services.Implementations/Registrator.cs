using Microsoft.Extensions.DependencyInjection;

namespace MusicManager.Domain.Services.Implementations
{
    public static class Registrator
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services) => services
            .AddTransient<ICueFileInteractor, CueFileInteractor>()
            .AddTransient<IPathToSongwriterService, DirectoryToSongwriterService>()
            .AddTransient<IPathToMovieService, DirectoryToMovieService>()
            .AddSingleton<IPathToCompilationService, DirectoryToCompilationService>()
            .AddSingleton<IPathToMovieReleaseService, DirectoryToMovieReleaseService>()
            .AddSingleton<IPathToSongService, FileToSongService>()
            .AddScoped<ISongwriterToFolderService, SongwriterToFolderService>()
            .AddScoped<IMovieToFolderService, MovieToFolderService>()
            .AddScoped<ICompilationToFolderService, CompilationToFolderService>()
            ;
    }
}
