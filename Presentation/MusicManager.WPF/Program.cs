using Microsoft.Extensions.Hosting;
using System;

namespace MusicManager.WPF;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        App app = new App();
        //app.InitializeComponent();
        app.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) => Host
            .CreateDefaultBuilder(args)
            .UseContentRoot(App.WorkingDirectory)
            .ConfigureServices(App.ConfigureServices);
}
