using MusicManager.Domain.Common;

namespace MusicManager.Repositories.Common;

public interface IRepository<T> where T : IAggregateRoot
{
}
