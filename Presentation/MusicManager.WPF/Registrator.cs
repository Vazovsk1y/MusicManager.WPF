using Microsoft.Extensions.DependencyInjection;
using MusicManager.Domain.Services;
using MusicManager.WPF.Tools;
using MusicManager.WPF.ViewModels;
using MusicManager.WPF.ViewModels.Entities;
using MusicManager.WPF.Views.Windows;

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
        .AddTransient<IRoot>(s =>
        {
            var configVm = s.GetRequiredService<UserConfigViewModel>();
            return configVm.CurrentConfig;
        })
        ;
}
