using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services.Implementations.Errors
{
    internal static class DomainServicesErrors
    {
        public static Error PassedDirectoryNamedIncorrect(string directoryPath) => new($"Passed directory [{directoryPath}] wasn't named correct.");
    }
}
