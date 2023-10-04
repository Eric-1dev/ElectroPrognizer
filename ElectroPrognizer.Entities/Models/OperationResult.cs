namespace ElectroPrognizer.Entities.Models;

public class OperationResult
{
    public bool IsSuccess { get; protected set; }
    public string Message { get; protected set; }
    public bool IsFail => !IsSuccess;

    protected OperationResult()
    { }

    public static OperationResult Success(string message = null)
    {
        return new OperationResult
        {
            IsSuccess = true,
            Message = message
        };
    }

    public static OperationResult Fail(string message)
    {
        return new OperationResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}

public class OperationResult<T> : OperationResult
{
    public T Entity { get; private set; }

    private OperationResult()
    { }

    public static OperationResult<T> Success(T entity, string message = null)
    {
        return new OperationResult<T>
        {
            IsSuccess = true,
            Message = message,
            Entity = entity
        };
    }

    public static OperationResult<T> Fail(string message)
    {
        return new OperationResult<T>
        {
            IsSuccess = false,
            Message = message
        };
    }
}
