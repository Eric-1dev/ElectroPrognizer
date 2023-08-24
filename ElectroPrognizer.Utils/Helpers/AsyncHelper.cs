namespace ElectroPrognizer.Utils.Helpers;

public static class AsyncHelper
{
    public static TResult ExecuteSync<TResult>(Func<Task<TResult>> action)
    {
        return action.Invoke().GetAwaiter().GetResult();
    }

    public static void ExecuteSync(Func<Task> action)
    {
        action.Invoke().GetAwaiter().GetResult();
    }

    public static void Wait(Task action)
    {
        action.GetAwaiter().GetResult();
    }
}
