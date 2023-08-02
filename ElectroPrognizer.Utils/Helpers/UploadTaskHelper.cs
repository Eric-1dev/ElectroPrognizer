using ElectroPrognizer.Utils.Models;

namespace ElectroPrognizer.Utils.Helpers;

public static class UploadTaskHelper
{
    public static int TotalCount { get; private set; }

    public static int CurrentIndex { get; private set; }

    public static bool IsFinished { get; private set; }

    public static bool IsCanceled { get; private set; }

    public static string Message { get; private set; }

    static UploadTaskHelper()
    {
        IsFinished = true;
    }

    public static void Init()
    {
        IsFinished = false;
        IsCanceled = false;
        CurrentIndex = 0;
        Message = null;
    }

    public static void SetTotalCount(int totalCount)
    {
        TotalCount = totalCount;
    }

    public static void IncrementCurrentIndex()
    {
        CurrentIndex++;
    }

    public static void SetToFinishedWithError(string error)
    {
        SetToFinished();
        Message = error;
    }

    public static void SetToFinished()
    {
        IsFinished = true;
        Message = "Импорт успешно завершен";
    }

    public static void SetMessage(string message)
    {
        Message = message;
    }

    public static void Cancel()
    {
        IsCanceled = true;
    }

    public static UploadProgressStatus GetProgressStatus()
    {
        int? percents = TotalCount == 0 || IsCanceled
            ? null
            : CurrentIndex * 100 / TotalCount;

        var message = Message;

        // сбрасываем сообщение для следующего запроса статуса
        Message = null;

        return UploadProgressStatus.Create(IsFinished, message, percents);
    }
}
