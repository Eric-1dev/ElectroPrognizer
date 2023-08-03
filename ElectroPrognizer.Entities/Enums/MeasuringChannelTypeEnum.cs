using System.ComponentModel;

namespace ElectroPrognizer.Entities.Enums;

public enum MeasuringChannelTypeEnum
{
    [Description("Активная энергия, прием")]
    ActiveInput = 1,

    [Description("Активная энергия, отдача")]
    ActiveOutput = 2,

    [Description("Реактивная энергия, прием")]
    ReactiveInput = 3,

    [Description("Реактивная энергия, отдача")]
    ReactiveOutput = 4
}
