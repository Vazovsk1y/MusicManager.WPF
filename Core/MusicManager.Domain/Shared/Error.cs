namespace MusicManager.Domain.Shared;

public class Error : IEquatable<Error>
{
    #region --Fields--

    public static readonly Error None = new(string.Empty);

    #endregion

    #region --Properties--

    public string Message { get; }

    #endregion

    #region --Constructors--

    public Error(string message)
    {
        Message = message;
    }

    #endregion

    #region --Methods--

    public static bool operator ==(Error? left, Error? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Error? left, Error? right) => !(left == right);

    public static implicit operator string(Error error) => error.Message;

    public bool Equals(Error? other) => Equals(other as object);

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

        if (obj is not Error error)
        {
            return false;
        }

        return error.Message == Message;
    }

    public override int GetHashCode() => Message.GetHashCode();

    #endregion
}
