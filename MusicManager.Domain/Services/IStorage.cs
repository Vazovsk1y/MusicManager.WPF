namespace MusicManager.Domain.Services;

public interface IStorage
{
    string Name { get; }

    string FullPath { get; }
}
