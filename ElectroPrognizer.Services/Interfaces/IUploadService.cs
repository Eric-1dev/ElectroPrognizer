namespace ElectroPrognizer.Services.Interfaces;

public interface IUploadService
{
    bool IsFinished { get; }

    bool IsCanceled { get; }

    void Init();

    void SetTotalCount(int totalCount);

    void IncrementCurrentIndex();

    void SendStatus(string message = null);

    void SetToFinishedWithError(string error);

    void SetToFinished(bool needToNotify);

    void Cancel();
}
