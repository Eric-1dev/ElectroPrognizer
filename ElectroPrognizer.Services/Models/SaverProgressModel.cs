namespace ElectroPrognizer.Services.Models;

public class SaverProgressModel
{
    public int TotalCount { get; set; }
    public int CurrentIndex { get; set; }
    public bool IsCompleted => CurrentIndex == TotalCount && IsInitialized;
    public bool IsInitialized => TotalCount != -1;

    public SaverProgressModel()
    {
        TotalCount = -1;
    }
}
