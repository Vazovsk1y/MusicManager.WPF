namespace MusicManager.Domain.Common;

public abstract class Entity
{
    #region --Fields--



    #endregion

    #region --Properties--

    public Guid Id { get; private init; }

    #endregion

    #region --Constructors--

    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    #endregion

    #region --Methods--

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        if (obj is not Entity entity)
        {
            return false;
        }

        return entity.Id == Id;
    }

    public override int GetHashCode() => Id.GetHashCode();

    #endregion
}
