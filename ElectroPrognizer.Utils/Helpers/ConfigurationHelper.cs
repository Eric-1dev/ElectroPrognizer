namespace ElectroPrognizer.Utils.Helpers;

public static class ConfigurationHelper
{
    public static string ConntectionString{ get; private set; }

    public static void SetConnectionString(string connectionString) => ConntectionString = connectionString;
}
