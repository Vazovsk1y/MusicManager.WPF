using Microsoft.Extensions.DependencyInjection;
using MusicManager.Services.Contracts.Factories;
using MusicManager.Services.Implementations.Contracts.Factories;

namespace MusicManager.Services.Implementations;

public static class Registrator
{
    public static IServiceCollection AddAppServices(this IServiceCollection services) => services
        .AddTransient<ISongwriterService, SongwriterService>()
        .AddTransient<IMovieService, MovieService>()
        .AddTransient<ICompilationService, CompilationService>()
        .AddTransient<IMovieReleaseService, MovieReleaseService>()
        .AddTransient<ISongService, SongService>()
        .AddSingleton<ISongwriterFolderFactory, SongwriterFolderFactory>()
        .AddSingleton<IMovieFolderFactory, MovieFolderFactory>()
        .AddSingleton<IDiscFolderFactory, DiscFolderFactory>()
        .AddSingleton<ISongFileFactory, SongFileFactory>()
        ;
}
