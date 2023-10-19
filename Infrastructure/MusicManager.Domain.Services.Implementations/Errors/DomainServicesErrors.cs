using MusicManager.Domain.Shared;

namespace MusicManager.Domain.Services.Implementations.Errors;

public static class DomainServicesErrors
{
    public static Error PassedDirectoryNamedIncorrect(string directoryPath) => new($"Passed directory [{directoryPath}] wasn't named correct.");

    public static Error PassedDirectoryIsNotExists(string directoryPath) => new($"Passed directory [{directoryPath}] is not exists.");

    public static readonly Error ParentAssociatedDirectoryIsNotCreated = new("Parent associated folder didn't create.");

    public static Error DirectoryForEntityIsAlreadyCreated(string entityName, string directoryPath) => new($"Directory {directoryPath} is already created or {entityName} with that folder info exists in database.");

    public static readonly Error AssociatedFolderIsNotCreated = new("Associated folder didn't create.");
}
