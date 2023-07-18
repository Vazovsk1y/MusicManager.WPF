using Microsoft.Extensions.Hosting;
using System;

namespace MusicManager.WPF;

internal class Program
{
    public static bool IsInDebug { get; private set; }

    [STAThread]
    public static void Main(string[] args)
    {

#if DEBUG
        IsInDebug = true;
#endif

        App app = new();
        app.InitializeComponent();
        app.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) => Host
            .CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((appConfig, _) =>
            {
                appConfig.HostingEnvironment.ApplicationName = App.Name;
                appConfig.HostingEnvironment.ContentRootPath = App.WorkingDirectory;
                appConfig.HostingEnvironment.EnvironmentName = IsInDebug ? "Development" : "Production";
            })
            .ConfigureServices(App.ConfigureServices);
}
