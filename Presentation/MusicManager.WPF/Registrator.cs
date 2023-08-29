using Microsoft.Extensions.DependencyInjection;
using MusicManager.Domain.Services;
using MusicManager.WPF.Tools;
using MusicManager.WPF.ViewModels;
using MusicManager.WPF.ViewModels.Entities;
using MusicManager.WPF.Views.Windows;
using System.IO;
using System.Text.Json;

namespace MusicManager.WPF;

internal static class Registrator
{
    public static IServiceCollection AddWPF(this IServiceCollection services) => services
        .AddWindowWithViewModelSingleton<MainWindow, MainWindowViewModel>()
        .AddWindowWithViewModelTransient<MovieAddWindow, MovieAddViewModel>()
        .AddWindowWithViewModelTransient<SongwriterAddWindow, SongwriterAddViewModel>()
        .AddWindowWithViewModelTransient<CompilationAddWindow, CompilationAddViewModel>()
        .AddWindowWithViewModelTransient<MovieReleaseAddWindow, MovieReleaseAddViewModel>()
        .AddWindowWithViewModelTransient<SongAddWindow, SongAddViewModel>()
        .AddSingleton<SongwirtersPanelViewModel>()
        .AddSingleton<MoviesPanelViewModel>()
        .AddSingleton<DiscsPanelViewModel>()
        .AddSingleton<SongsPanelViewModel>()
        .AddSingleton<UserConfigViewModel>()
        .AddTransient<MovieReleaseAddToMovieViewModel>()
        .AddTransient<MovieReleaseMovieWindow>()
        .AddSingleton(typeof(IWpfWindowService<>), typeof(WpfWindowService<>))
        .AddTransient<IUserConfig, UserConfig>(_ =>
        {
			var fileInfo = new FileInfo(UserConfig.SettingsFileFullPath);
			if (!fileInfo.Exists)
			{
				return UserConfig.Default;
			}

			using var stream = fileInfo.OpenRead();
			var config = JsonSerializer.Deserialize<UserConfig>(stream) ?? UserConfig.Default;
			return config;
		})
        .AddTransient<IRoot>(s =>
        {
            return s.GetRequiredService<IUserConfig>();
		})
        ;
}
