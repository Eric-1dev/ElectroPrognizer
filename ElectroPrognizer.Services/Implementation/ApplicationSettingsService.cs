using ElectroPrognizer.DataLayer;
using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Entities.Enums;
using ElectroPrognizer.Entities.Models;
using ElectroPrognizer.Services.Interfaces;

namespace ElectroPrognizer.Services.Implementation;

public class ApplicationSettingsService : IApplicationSettingsService
{
    private const string NotFoundErrorMessage = "Не найдена указанная настройка";

    public ApplicationSetting[] GetAll()
    {
        using var dbContext = new ApplicationContext();

        return dbContext.ApplicationSettings.ToArray();
    }
    public ApplicationSetting Get(ApplicationSettingEnum applicationSettingType)
    {
        using var dbContext = new ApplicationContext();

        var setting = dbContext.ApplicationSettings.FirstOrDefault(x => x.ApplicationSettingType == applicationSettingType);

        if (setting == null)
            throw new ArgumentOutOfRangeException(nameof(applicationSettingType), NotFoundErrorMessage);

        return setting;
    }

    public OperationResult SetValue(ApplicationSettingEnum applicationSettingType, string value)
    {
        try
        {
            using var dbContext = new ApplicationContext();

            var setting = dbContext.ApplicationSettings.FirstOrDefault(x => x.ApplicationSettingType == applicationSettingType);

            if (setting == null)
                throw new ArgumentOutOfRangeException(nameof(setting), NotFoundErrorMessage);

            setting.Value = value;

            dbContext.SaveChanges();

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.ToString());
        }
    }

    public string GetStringValue(ApplicationSettingEnum applicationSettingType)
    {
        using var dbContext = new ApplicationContext();

        var setting = dbContext.ApplicationSettings.FirstOrDefault(x => x.ApplicationSettingType == applicationSettingType);

        if (setting == null)
            throw new ArgumentOutOfRangeException(nameof(setting), "Не найдена указанная настройка");

        return setting.Value;
    }

    public int GetIntValue(ApplicationSettingEnum applicationSettingType)
    {
        var stringValue = GetStringValue(applicationSettingType);

        var isSuccess = int.TryParse(stringValue, out int value);

        if (isSuccess)
            return value;

        throw new FormatException("Тип значения не соответствует типу int");
    }

    public double GetDoubleValue(ApplicationSettingEnum applicationSettingType)
    {
        var stringValue = GetStringValue(applicationSettingType);

        var isSuccess = double.TryParse(stringValue, out double value);

        if (isSuccess)
            return value;

        throw new FormatException("Тип значения не соответствует типу double");
    }
}
