using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services.Implementations
{
    public class FilePathIsValidService : IPathIsValidService<FileInfo>
    {
        public Result<FileInfo> IsValidPath(string path)
        {
            try
            {
                var fileInfo = new FileInfo(path);
                return Result.Success(fileInfo);
            }
            catch (ArgumentException)
            {
                return Result.Failure<FileInfo>(new Error($"Passed {path} file path wasn't correct."));
            }
            catch (PathTooLongException)
            {
                return Result.Failure<FileInfo>(new Error($"Passed {path} file path was too long."));
            }
            catch (Exception ex)
            {
                return Result.Failure<FileInfo>(new Error($"{ex.Message}"));
            }
        }
    }
}
