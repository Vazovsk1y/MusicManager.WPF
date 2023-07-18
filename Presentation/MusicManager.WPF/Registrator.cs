using Microsoft.Extensions.DependencyInjection;
using MusicManager.WPF.Tools;
using MusicManager.WPF.ViewModels;

namespace MusicManager.WPF;

internal static class Registrator
{
    public static IServiceCollection AddWPF(this IServiceCollection services) => services
        .AddWindowWithViewModelSingleton<MainWindow, MainWindowViewModel>();
}
