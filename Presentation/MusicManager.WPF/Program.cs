using Microsoft.Extensions.Hosting;
using MusicManager.Utils;
using MusicManager.WPF.Tools;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Threading;

namespace MusicManager.WPF;

internal class Program
{
    public static bool IsInDebug { get; private set; }

    private static Mutex? _mutex;

    public const double REGEX_DEFAULT_MATCH_TIMEOUT_Milliseconds = 50d;

    [STAThread]
    public static void Main(string[] args)
    {
        _mutex = new Mutex(true, App.Name, out bool createdNew);
        if (!createdNew)
        {
            MessageBoxHelper.ShowErrorBox("App is already running.");
            return;
        }

        AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(REGEX_DEFAULT_MATCH_TIMEOUT_Milliseconds));

#if DEBUG
        IsInDebug = true;
#endif

        App app = new();
        app.StartGlobalExceptionsHandling();
        app.InitializeComponent();
		app.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", IsInDebug ? "Development" : "Production");

        return Host
            .CreateDefaultBuilder(args)
            .CreateAssociatedFolderInAppData()
            .UseSerilog((host, loggingConfiguration) =>
            {
                if (host.HostingEnvironment.IsDevelopment())
                {
                    loggingConfiguration.MinimumLevel.Debug();
                    loggingConfiguration.WriteTo.Debug();
                }
                else
                {
					loggingConfiguration.MinimumLevel.Information();
                    loggingConfiguration.MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Error);

					string logFileName = "log.txt";
                    string logsStoragePath = Path.Combine(App.AssociatedAppFolderFullPath, "logs");
                    string logFileFullPath = Path.Combine(logsStoragePath, logFileName);

					DirectoryHelper.CreateIfNotExists(logsStoragePath);
                    loggingConfiguration.WriteTo.File(logFileFullPath, rollingInterval: RollingInterval.Hour);
				}
            })
            .ConfigureAppConfiguration((appConfig, _) =>
            {
                appConfig.HostingEnvironment.ApplicationName = App.Name;
                appConfig.HostingEnvironment.ContentRootPath = App.WorkingDirectory;
            })
            .ConfigureServices(App.ConfigureServices);
    }
}
