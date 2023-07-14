using Microsoft.Extensions.DependencyInjection;
using MusicManager.WPF.ViewModels;

namespace MusicManager.WPF;

internal static class Registrator
{
    public static void AddWPF(this IServiceCollection services) => services
        .AddWindowWithViewModelSingleton<MainWindow, MainWindowViewModel>();
}
