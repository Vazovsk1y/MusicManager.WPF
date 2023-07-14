using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System;
using MusicManager.WPF.ViewModels.Base;
using MusicManager.WPF.ViewModels;

namespace MusicManager.WPF;

internal static class Registrator
{
    public static void AddWPF(this IServiceCollection services) => services
        .AddWindowSingleton<MainWindow, MainWindowViewModel>();
}

internal static class GenericWindowRegistrator
{
    public static IServiceCollection AddWindowTransient<TWindow, TViewModel>(this IServiceCollection services)
            where TViewModel : ViewModel
            where TWindow : Window
            => services.AddTransient(s =>
            {
                var viewModel = s.GetRequiredService<TViewModel>();
                var window = Activator.CreateInstance<TWindow>();
                window.DataContext = viewModel;
                return window;
            });

    public static IServiceCollection AddWindowSingleton<TWindow, TViewModel>(this IServiceCollection services)
            where TViewModel : ViewModel
            where TWindow : Window
            => services.AddSingleton(s =>
            {
                var viewModel = s.GetRequiredService<TViewModel>();
                var window = Activator.CreateInstance<TWindow>();
                window.DataContext = viewModel;
                return window;
            });
}

