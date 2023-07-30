namespace ElectroPrognizer.Utils.Helpers
{
    public static class CancellationTokenHelper
    {
        private static readonly CancellationTokenSource _cancellationTokenSource = new();

        public static CancellationToken Token => _cancellationTokenSource.Token;

        public static void Cancel() => _cancellationTokenSource.Cancel();
    }
}
