using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services.Implementations
{
    public class DirectoryPathIsValidService : IPathIsValidService<DirectoryInfo>
    {
        public Result<DirectoryInfo> IsValidPath(string path)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(path);
                return Result.Success(directoryInfo);
            }
            catch (ArgumentException)
            {
                return Result.Failure<DirectoryInfo>(new Error($"Passed {path} directory path wasn't correct."));
            }
            catch (PathTooLongException)
            {
                return Result.Failure<DirectoryInfo>(new Error($"Passed {path} directory path was too long."));
            }
            catch (Exception ex)
            {
                return Result.Failure<DirectoryInfo>(new Error($"{ex.Message}"));
            }
        }
    }
}
