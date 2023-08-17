using ElectroPrognizer.DataLayer;
using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Entities.Enums;
using ElectroPrognizer.Services.Interfaces;

namespace ElectroPrognizer.Services.Implementation;

public class DbLogService : IDbLogService
{
    public void LogError(string message, Exception ex = null)
    {
        Log(LogLevelEnum.Error, message, ex);
    }

    public void LogInfo(string message, Exception ex = null)
    {
        Log(LogLevelEnum.Information, message, ex);
    }

    public void LogWarning(string message, Exception ex = null)
    {
        Log(LogLevelEnum.Warning, message, ex);
    }

    private void Log(LogLevelEnum logLevel, string message, Exception ex = null)
    {
        using var dbContext = new ApplicationContext();

        var logEntity = new DbLogEntity
        {
            LogLevel = logLevel,
            Message = ex == null ? message : $"{message}. Исключение: {ex}",
        };

        dbContext.Logs.Add(logEntity);

        dbContext.SaveChanges();
    }
}
