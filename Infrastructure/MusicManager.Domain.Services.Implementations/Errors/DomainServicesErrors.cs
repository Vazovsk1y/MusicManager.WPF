using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services.Implementations.Errors
{
    public static class DomainServicesErrors
    {
        public static Error PassedDirectoryNamedIncorrect(string directoryPath) => new($"Passed directory [{directoryPath}] wasn't named correct.");

        public static Error PassedDirectoryIsNotExists(string directoryPath) => new($"Passed directory [{directoryPath}] is not exists.");

        public static Error PassedDirectoryPathIsInvalid(string directoryPath) => new($"Passed directory path [{directoryPath}] is invalid.");

        public static Error PassedFileNamedIncorrect(string directoryPath) => new($"Passed file [{directoryPath}] wasn't named correct.");

        public static Error PassedFileIsNotExists(string directoryPath) => new($"Passed file [{directoryPath}] is not exists.");

        public static Error PassedFilePathIsInvalid(string directoryPath) => new($"Passed file path [{directoryPath}] is invalid.");
    }
}
