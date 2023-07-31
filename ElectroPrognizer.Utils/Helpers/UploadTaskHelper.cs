using ElectroPrognizer.Utils.Models;

namespace ElectroPrognizer.Utils.Helpers;

public static class UploadTaskHelper
{
    private static readonly object _locker = new();
    private static bool _isInProgress = false;
    private static int _totalCount;
    private static int _currentIndex;
    private static bool _isCanceled;

    public static bool IsCompleted => !_isInProgress;

    public static bool StartUpload(int totalCount)
    {
        if (_isInProgress)
            return false;

        lock (_locker)
        {
            if (_isInProgress)
                return false;

            _isInProgress = true;
            _isCanceled = false;
            _totalCount = totalCount;
            _currentIndex = 0;

            return true;
        }
    }

    public static void IncrementCurrentIndex()
    {
        _currentIndex++;
    }

    public static void FinishUpload()
    {
        _isInProgress = false;
    }

    public static void Cancel()
    {
        _isCanceled = true;
    }

    public static bool IsCanceled()
    {
        return _isCanceled;
    }

    public static UploadProgressStatus GetProgressStatus()
    {
        int? percents = null;

        if (_totalCount != 0)
            percents = _currentIndex * 100 / _totalCount;

        //return UploadProgressStatus.Create(IsCompleted, percents);
        return UploadProgressStatus.Create(false, 59);
    }
}
