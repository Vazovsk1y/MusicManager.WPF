using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicManager.DAL;
using MusicManager.Domain.Services.Implementations;
using MusicManager.Repositories.Data;
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

    public const string Name = "MusicManager";

    #endregion

    #region --Properties--

    public static string WorkingDirectory => IsInDesignMode ? Path.GetDirectoryName(GetSourceCodePath())! : Environment.CurrentDirectory;

    public static bool IsInDesignMode { get; private set; } = true;

    public static IServiceProvider Services => Host.Services;

    public static IHost Host => _host ??= Program.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

    #endregion

    #region --Constructors--



    #endregion

    #region --Methods--

    internal static void ConfigureServices(HostBuilderContext hostBuilder, IServiceCollection services) => services
        .AddWPF()
        .AddDAL(hostBuilder.Configuration.GetSection("Database"))
        .AddDomainServices()
        .AddAppServices()
        ;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        IsInDesignMode = false;
        await Host.StartAsync();

        using var scope = Services.CreateScope();
        var databaseInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        await databaseInitializer.InitializeAsync();

        Services.GetRequiredService<MainWindow>().Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        using var host = Host;
        await host.StopAsync();
    }

    private static string GetSourceCodePath([CallerFilePath] string path = null) => string.IsNullOrWhiteSpace(path) ?
        throw new ArgumentNullException(nameof(path)) : path;

    #endregion
}
