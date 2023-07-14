using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicManager.DAL;
using MusicManager.Domain.Services.Implementations;
using MusicManager.Services.Implementations;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MusicManager.WPF;

public partial class App : Application
{
    #region --Fields--

    private static IHost? _host;

    #endregion

    #region --Properties--

    public static string WorkingDirectory => IsDesignMode ? Path.GetDirectoryName(GetSourceCodePath())! : Environment.CurrentDirectory;

    public static bool IsDesignMode { get; private set; } = true;

    public static IServiceProvider Services => Host.Services;

    public static IHost Host => _host ??= Program.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

    #endregion

    #region --Constructors--



    #endregion

    #region --Methods--

    internal static void ConfigureServices(HostBuilderContext host, IServiceCollection services) => services
        .AddWPF()
        .AddDAL()
        .AddDomainServices()
        .AddAppServices()
        ;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        IsDesignMode = false;
        await Host.StartAsync();
        Services.GetRequiredService<MainWindow>().Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        await Host.StopAsync();
        Host.Dispose();
    }

    private static string GetSourceCodePath([CallerFilePath] string path = null) => string.IsNullOrWhiteSpace(path) ?
        throw new ArgumentNullException(nameof(path)) : path;

    #endregion
}
