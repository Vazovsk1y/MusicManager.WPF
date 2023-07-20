using MusicManager.Domain.Shared;
using MusicManager.Services.Contracts.Base;

namespace MusicManager.Services.Contracts.Factories;

public interface IDiscFolderFactory
{
    Result<DiscFolder> Create(string discFolderPath);
}


