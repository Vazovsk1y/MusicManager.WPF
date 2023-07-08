namespace MusicManager.Services.Contracts.Base;

public interface IDiscFolder
{
    string Path { get; }

    IEnumerable<ISongFile> Songs { get; }

    IEnumerable<string> CoversPaths { get; }
}
