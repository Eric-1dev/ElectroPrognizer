using ElectroPrognizer.DataLayer;
using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Entities.Enums;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models;
using ElectroPrognizer.Utils.Constants;

namespace ElectroPrognizer.Services.Implementation;

public class DownloadLogService : IDownloadLogService
{
    public DownloadLogResult GetLogs(DownloadLogFilter filter)
    {
        using var dbContext = new ApplicationContext();

        var dateFrom = filter.DateFrom?.Date ?? DateTime.MinValue;
        var dateTo = filter.DateTo?.Date.AddDays(1) ?? DateTime.MaxValue;

        var pageSize = DownloadLogConstants.PageSize;

        var skipCount = (filter.PageNumber - 1) * pageSize;

        var entities = dbContext.DownloadLogs
            .Where(x => x.Created >= dateFrom && x.Created < dateTo)
            .Skip(skipCount)
            .Take(pageSize)
            .ToArray();

        var result = new DownloadLogResult
        {
            TotalPages = (entities.Length + pageSize - 1) / pageSize,
            Entities = entities
        };

        return result;
    }

    public int GetTotalCount()
    {
        using var dbContext = new ApplicationContext();

        return dbContext.DownloadLogs.Count();
    }

    public void LogError(string message, Exception ex = null)
    {
        Log(LogLevelEnum.Error, message, ex);
    }

    public void LogInfo(string message)
    {
        Log(LogLevelEnum.Information, message);
    }

    private void Log(LogLevelEnum logLevel, string message, Exception ex = null)
    {
        using var dbContext = new ApplicationContext();

        var logEntity = new DownloadLogEntity
        {
            LogLevel = logLevel,
            Message = ex == null ? message : $"{message}. Исключение: {ex}",
        };

        dbContext.DownloadLogs.Add(logEntity);

        dbContext.SaveChanges();
    }
}
