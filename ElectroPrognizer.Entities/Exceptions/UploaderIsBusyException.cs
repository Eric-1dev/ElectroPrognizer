namespace ElectroPrognizer.Entities.Exceptions;

public class UploaderIsBusyException : Exception
{
    public UploaderIsBusyException(string message) : base(message)
    { }
}
