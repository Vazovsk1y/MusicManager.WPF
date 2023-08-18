using MusicManager.Domain.Services;
using MusicManager.Services.Contracts.Base;

namespace MusicManager.Services.Contracts;

public interface IAppConfig : IConfig, IRoot
{
    bool DeleteAssociatedFolder { get; set; }
}
