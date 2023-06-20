namespace MusicManager.Domain.Shared;

public class Result
{
    #region --Fields--



    #endregion

    #region --Properties--

    public bool IsFailure => !IsSuccess;

    public bool IsSuccess { get; }

    public Error Error { get; }

    #endregion

    #region --Constructors--

    protected internal Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    #endregion

    #region --Methods--

    public static Result Success() => new(true, Error.None);

    public static Result<T> Success<T>(T value) => new(value, true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<T> Failure<T>(Error error) => new(default, false, error);

    #endregion
}
