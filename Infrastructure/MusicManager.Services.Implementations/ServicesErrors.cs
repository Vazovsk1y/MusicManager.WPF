using MusicManager.Domain.Shared;

namespace MusicManager.Services.Implementations;

internal static class ServicesErrors
{
    public static Error SongwriterWithPassedIdIsNotExists() => new("Songwriter with passed id is not exists in database.");

    public static Error MovieWithPassedIdIsNotExists() => new("Movie with passed id is not exists in database.");

    public static Error CompilationWithPassedIdIsNotExists() => new("Compilation with passed id is not exists in database.");

    public static Error MovieReleaseWithPassedIdIsNotExists() => new("Movie release with passed id is not exists in database.");

    public static Error SongWithPassedIdIsNotExists() => new("Song with passed id is not exists in database.");

    public static Error DiscWithPassedIdIsNotExists() => new("Disc with passed id is not exists in database.");
}
