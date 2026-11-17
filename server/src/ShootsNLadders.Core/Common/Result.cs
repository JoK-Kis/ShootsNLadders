namespace ShootsNLadders.Core.Common;

/// <summary>
/// The outcome of an operation: either a value (success) or an <see cref="Common.Error"/> (failure).
/// Used instead of exceptions for expected failures like "entry already exists".
/// </summary>
public sealed class Result<T>
{

    private readonly T? _value;

    public bool IsSuccess { get; }

    /// <summary>The error when the operation failed; null on success.</summary>
    public Error? Error { get; }

    /// <summary>The value on success. Throws if read on a failed result, since that is a programming bug.</summary>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot read the value of a failed result.");


    private Result(T value)
    {
        IsSuccess = true;
        _value = value;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(Error error) => new(error);
}
