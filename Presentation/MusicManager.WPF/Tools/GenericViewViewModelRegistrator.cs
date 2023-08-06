using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MusicManager.WPF.Tools;

internal static class GenericViewViewModelRegistrator
{
    public static IServiceCollection AddWindowWithViewModelTransient<TWindow, TViewModel>(this IServiceCollection services)
            where TViewModel : ObservableObject
            where TWindow : Window
            => services
        .AddTransient<TViewModel>()
        .AddTransient(s =>
            {
                var viewModel = s.GetRequiredService<TViewModel>();

                if (viewModel is ObservableRecipient observableRecipient)
                {
                    observableRecipient.IsActive = true;
                }

                var window = Activator.CreateInstance<TWindow>();
                window.DataContext = viewModel;
                return window;
            });

    public static IServiceCollection AddWindowWithViewModelSingleton<TWindow, TViewModel>(this IServiceCollection services)
            where TViewModel : ObservableObject
            where TWindow : Window
            => services
        .AddSingleton<TViewModel>()
        .AddSingleton(s =>
            {
                var viewModel = s.GetRequiredService<TViewModel>();
                var window = Activator.CreateInstance<TWindow>();
                window.DataContext = viewModel;
                return window;
            });
}

