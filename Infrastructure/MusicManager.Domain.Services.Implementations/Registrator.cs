using Microsoft.Extensions.DependencyInjection;

namespace MusicManager.Domain.Services.Implementations
{
    public static class Registrator
    {
        public static void AddDomainServices(this IServiceCollection services) => services
            .AddTransient<ICueFileInteractor, CueFileInteractor>()
            .AddTransient<IPathToSongwriterService, DirectoryToSongwriterService>()
            .AddTransient<IPathToMovieService, DirectoryToMovieService>()
            .AddTransient<IPathToCompilationService, DirectoryToCompilationService>()
            .AddTransient<IPathToMovieReleaseService, DirectoryToMovieReleaseService>()
            .AddTransient<IPathToSongService, FileToSongService>()
            ;
    }
}
