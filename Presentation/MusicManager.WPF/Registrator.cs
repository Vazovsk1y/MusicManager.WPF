using Microsoft.Extensions.DependencyInjection;
using MusicManager.Utils;
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
        .AddSingleton<SongwirtersPanelViewModel>()
        .AddSingleton<MoviesPanelViewModel>()
        .AddSingleton<DiscsPanelViewModel>()
        .AddSingleton<SongsPanelViewModel>()
        .AddSingleton<ICountriesHelper, CountryHelper>()
        .AddSingleton<IYearsHelper, YearsHelper>()
        ;
}
