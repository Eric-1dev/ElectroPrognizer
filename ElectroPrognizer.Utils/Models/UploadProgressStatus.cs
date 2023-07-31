namespace ElectroPrognizer.Utils.Models;

public class UploadProgressStatus
{
    public bool IsComplete { get; private set; }
    public int? Percents { get; private set; }

    private UploadProgressStatus()
    { }

    public static UploadProgressStatus Create(bool isComplete, int? percents)
    {
        return new UploadProgressStatus { IsComplete = isComplete, Percents = percents };
    }
}
