using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MusicManager.WPF.Tools;

public class WpfWindowService<TWindow> : IWpfWindowService<TWindow> where TWindow : Window
{
    private TWindow? _window;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WpfWindowService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public void StartDialog<TViewModel>(TViewModel dataContext) where TViewModel : ObservableObject
    {
        CloseDialog();

        var scope = _serviceScopeFactory.CreateScope();
        _window = scope.ServiceProvider.GetRequiredService<TWindow>();
        _window.DataContext = dataContext;
        _window.Closed += (_, _) => scope.Dispose();
        _window.ShowDialog();
    }

    public void CloseDialog()
    {
        _window?.Close();
        _window = null;
    }
}