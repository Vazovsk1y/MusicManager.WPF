using Microsoft.Extensions.DependencyInjection;
using MusicManager.Domain.Services;
using MusicManager.Services.Contracts;
using MusicManager.Services.Contracts.Factories;
using MusicManager.Services.Implementations.Contracts.Factories;
using System.IO;
using System.Text.Json;

namespace MusicManager.Services.Implementations;

public static class Registrator
{
    public static IServiceCollection AddAppServices(this IServiceCollection services) => services
        .AddTransient<ISongwriterService, SongwriterService>()
        .AddTransient<IMovieService, MovieService>()
        .AddTransient<ICompilationService, CompilationService>()
        .AddTransient<IMovieReleaseService, MovieReleaseService>()
        .AddTransient<ISongService, SongService>()
        .AddTransient<IBaseDiscService, BaseDiscService>()
        .AddTransient<IFileManagerInteractor, FileManagerInteractor>()
        .AddSingleton<ISongwriterFolderFactory, SongwriterFolderFactory>()
        .AddSingleton<IMovieFolderFactory, MovieFolderFactory>()
        .AddSingleton<IDiscFolderFactory, DiscFolderFactory>()
        .AddSingleton<ISongFileFactory, SongFileFactory>()
        .AddSingleton(typeof(IUserDialogService<>), typeof(BaseUserDialogService<>))
        .AddTransient<IAppConfig, AppConfig>(_ =>
        {
            var fileInfo = new FileInfo(AppConfig.FullPath);
            if (!fileInfo.Exists)
            {
                return AppConfig.Default;
            }

            using var stream = fileInfo.OpenRead();
            return JsonSerializer.Deserialize<AppConfig>(stream) ?? AppConfig.Default;
        })
        .AddTransient<IRoot>(s =>
        {
            return s.GetRequiredService<IAppConfig>();
        })
        ;
}
