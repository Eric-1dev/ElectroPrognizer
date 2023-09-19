using ElectroPrognizer.Services.Hubs;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Utils.Helpers;
using ElectroPrognizer.Utils.Models;
using Microsoft.AspNetCore.SignalR;

namespace ElectroPrognizer.Services.Implementation;

public class UploadService : IUploadService
{
    private int _totalCount;
    private int _currentIndex;

    public bool IsFinished { get; private set; }
    public bool IsCanceled { get; private set; }

    public IHubContext<StatusHub> StatusHub { get; set; }

    public UploadService()
    {
        IsFinished = true;
    }

    public void Init()
    {
        IsFinished = false;
        IsCanceled = false;
        _currentIndex = 0;

        AsyncHelper.Wait(StatusHub.Clients.All.SendAsync("ReceiveStatus", GetProgressStatus()));
    }

    public void SetTotalCount(int totalCount)
    {
        _totalCount = totalCount;
    }

    public void IncrementCurrentIndex()
    {
        _currentIndex++;

        var coefficient = _currentIndex * 1000 / _totalCount;

        if (coefficient % 10 != 0)
            return;

        AsyncHelper.Wait(StatusHub.Clients.All.SendAsync("ReceiveStatus", GetProgressStatus(message: null, percents: coefficient / 10)));
    }

    public void SendStatus(string message = null)
    {
        AsyncHelper.Wait(StatusHub.Clients.All.SendAsync("ReceiveStatus", GetProgressStatus(message)));
    }

    public void SetToFinishedWithError(string error)
    {
        SetToFinished(error, needToNotify: true);
    }

    public void SetToFinished(bool needToNotify)
    {
        SetToFinished("Импорт успешно завершен", needToNotify);
    }

    public void Cancel()
    {
        IsCanceled = true;
        AsyncHelper.Wait(StatusHub.Clients.All.SendAsync("ReceiveStatus", GetProgressStatus()));
    }

    private UploadProgressStatus GetProgressStatus(string message = null, int? percents = null)
    {
        return UploadProgressStatus.Create(IsFinished, message, percents);
    }

    private void SetToFinished(string message, bool needToNotify)
    {
        IsFinished = true;

        if (needToNotify)
            AsyncHelper.Wait(StatusHub.Clients.All.SendAsync("ReceiveStatus", GetProgressStatus(message)));
    }
}
