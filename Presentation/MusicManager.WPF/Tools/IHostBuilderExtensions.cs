using Microsoft.Extensions.Hosting;
using MusicManager.Utils;
using System.IO;

namespace MusicManager.WPF.Tools;

public static class IHostBuilderExtensions
{
    public static IHostBuilder CreateAssociatedFolderInAppData(this IHostBuilder hostBuilder)
    {
        string companyFolder = Path.Combine(DirectoryHelper.LocalApplicationDataPath, App.CompanyName);
        DirectoryHelper.CreateIfNotExists(companyFolder);
        DirectoryHelper.CreateIfNotExists(App.AssociatedAppFolderFullPath);
        return hostBuilder;
    }
}
