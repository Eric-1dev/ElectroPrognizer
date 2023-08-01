namespace ElectroPrognizer.Utils.Models;

public class UploadProgressStatus
{
    public bool IsFinished { get; private set; }
    public int? Percents { get; private set; }
    public string Message { get; private set; }

    private UploadProgressStatus()
    { }

    public static UploadProgressStatus Create(bool isFinished, string message, int? percents)
    {
        return new UploadProgressStatus
        {
            IsFinished = isFinished,
            Message = message,
            Percents = percents
        };
    }
}
