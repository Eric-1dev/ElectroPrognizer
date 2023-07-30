using ElectroPrognizer.DataModel.Models;

namespace ElectroPrognizer.Utils.Helpers
{
    public static class ConfigurationHelper
    {
        public static AppConfiguration? Config { get; private set; }
        public static string? ConntectionString{ get; private set; }

        public static void SetConfiguration(AppConfiguration config) => Config = config;

        public static void SetConnectionString(string? connectionString) => ConntectionString = connectionString;
    }
}