using Microsoft.Extensions.DependencyInjection;
using MusicManager.DAL.Repositories;
using MusicManager.Repositories;
using MusicManager.Repositories.Data;

namespace MusicManager.DAL;

public static class Registrator
{
    public static void AddDAL(this IServiceCollection services) => services
        .AddScoped<ISongwriterRepository, SongwriterRepository>()
        .AddScoped<IMovieRepository, MovieRepository>()
        .AddScoped<ICompilationRepository, CompilationRepository>()
        .AddScoped<IMovieReleaseRepository, MovieReleaseRepository>()
        .AddScoped<ISongRepository, SongRepository>()
        .AddScoped<IUnitOfWork, UnitOfWork>()
        ;
}
