namespace ElectroPrognizer.SchedulerServices.Helpers;

public static class ReceiveEmailJobSemaphoreHelper
{
    private static readonly SemaphoreSlim _semaphore;

    static ReceiveEmailJobSemaphoreHelper()
    {
        _semaphore = new SemaphoreSlim(1, 1);
    }

    public static SemaphoreSlim GetSemaphore() => _semaphore;
}
