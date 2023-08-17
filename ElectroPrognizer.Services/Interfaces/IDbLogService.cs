namespace ElectroPrognizer.Services.Interfaces;

public interface IDbLogService
{
    void LogInfo(string message, Exception ex = null);

    void LogWarning(string message, Exception ex = null);

    void LogError(string message, Exception ex = null);
}
