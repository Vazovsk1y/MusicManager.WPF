using Microsoft.Extensions.DependencyInjection;

namespace MusicManager.Services.Implementations;

public static class Registrator
{
    public static IServiceCollection AddAppServices(this IServiceCollection services) => services
        .AddTransient<ISongwriterService, SongwriterService>()
        .AddTransient<IMovieService, MovieService>()
        .AddTransient<ICompilationService, CompilationService>()
        .AddTransient<IMovieReleaseService, MovieReleaseService>()
        .AddTransient<ISongService, SongService>()
        ;
}
