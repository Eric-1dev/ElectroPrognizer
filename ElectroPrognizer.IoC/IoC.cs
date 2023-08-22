using Autofac;
using ElectroPrognizer.Services.Implementation;
using ElectroPrognizer.Services.Interfaces;

namespace ElectroPrognizer.IoC
{
    public static class IoC
    {
        public static void InitContainer(this ContainerBuilder builder)
        {
            builder.RegisterType<ApplicationSettingsService>()
                .As<IApplicationSettingsService>()
                .SingleInstance()
                .PropertiesAutowired();

            builder.RegisterType<ExcelConsumptionReader>()
                .As<IExcelConsumptionReader>()
                .SingleInstance()
                .PropertiesAutowired();
            
            builder.RegisterType<XmlReaderService>()
                .As<IXmlReaderService>()
                .SingleInstance()
                .PropertiesAutowired();
            
            builder.RegisterType<EnergyConsumptionSaverService>()
                .As<IEnergyConsumptionSaverService>()
                .SingleInstance()
                .PropertiesAutowired();
            
            builder.RegisterType<ImportFileService>()
                .As<IImportFileService>()
                .SingleInstance()
                .PropertiesAutowired();

            builder.RegisterType<SubstationService>()
                .As<ISubstationService>()
                .SingleInstance()
                .PropertiesAutowired();

            builder.RegisterType<ElectricityMeterService>()
                .As<IElectricityMeterService>()
                .SingleInstance()
                .PropertiesAutowired();

            builder.RegisterType<PrognizerService>()
                .As<IPrognizerService>()
                .SingleInstance()
                .PropertiesAutowired();

            builder.RegisterType<DayReportService>()
                .As<IDayReportService>()
                .SingleInstance()
                .PropertiesAutowired();

            builder.RegisterType<EmailService>()
                .As<IEmailService>()
                .SingleInstance()
                .PropertiesAutowired();

            builder.RegisterType<DownloadLogService>()
                .As<IDownloadLogService>()
                .SingleInstance()
                .PropertiesAutowired();
        }
    }
}
