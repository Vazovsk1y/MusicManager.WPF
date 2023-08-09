using Microsoft.Extensions.Hosting;
using System;

namespace MusicManager.WPF;

internal class Program
{
    public static bool IsInDebug { get; private set; }

    public const double REGEX_DEFAULT_MATCH_TIMEOUT_Milliseconds = 50d;

    [STAThread]
    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(REGEX_DEFAULT_MATCH_TIMEOUT_Milliseconds));

#if DEBUG
        IsInDebug = true;
#endif

        App app = new();
        app.InitializeComponent();
        app.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", IsInDebug ? "Development" : "Production");

        return Host
            .CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((appConfig, _) =>
            {
                appConfig.HostingEnvironment.ApplicationName = App.Name;
                appConfig.HostingEnvironment.ContentRootPath = App.WorkingDirectory;
            })
            .ConfigureServices(App.ConfigureServices);
    }
}
