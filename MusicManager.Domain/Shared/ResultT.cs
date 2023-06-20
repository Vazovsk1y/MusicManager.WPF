namespace MusicManager.Domain.Shared;

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error)
        => _value = value;

    public TValue Value => IsFailure ? 
        throw new InvalidOperationException("The value of failed result can't be accessed.") 
        : 
        _value!;

    public static implicit operator Result<TValue>(TValue value) => new(value, true, Error.None);
}
