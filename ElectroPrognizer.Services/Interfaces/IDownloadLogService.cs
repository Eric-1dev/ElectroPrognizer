using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Services.Models;

namespace ElectroPrognizer.Services.Interfaces;

public interface IDownloadLogService
{
    void LogInfo(string message);

    void LogError(string message, Exception ex = null);

    int GetTotalCount();

    DownloadLogEntity[] GetLogs(DownloadLogFilter filter);
}
