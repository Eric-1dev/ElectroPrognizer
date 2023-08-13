using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Entities.Enums;
using ElectroPrognizer.Entities.Models;

namespace ElectroPrognizer.Services.Interfaces;

public interface IApplicationSettingsService
{
    ApplicationSetting[] GetAll();

    ApplicationSetting Get(ApplicationSettingEnum applicationSettingType);

    OperationResult SetValue(ApplicationSettingEnum applicationSettingType, string value);

    string GetStringValue(ApplicationSettingEnum applicationSettingType);

    int GetIntValue(ApplicationSettingEnum applicationSetting);
}
