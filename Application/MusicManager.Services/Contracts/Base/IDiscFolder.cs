namespace MusicManager.Services.Contracts.Base;

public interface IDiscFolder
{
    string Path { get; }

    IReadOnlyCollection<ISongFile> Songs { get; }

    IReadOnlyCollection<string> CoversPaths { get; }
}
