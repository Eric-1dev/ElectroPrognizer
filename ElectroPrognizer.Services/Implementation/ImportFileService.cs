using ElectroPrognizer.Entities.Exceptions;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models;
using ElectroPrognizer.Utils.Helpers;

namespace ElectroPrognizer.Services.Implementation;

public class ImportFileService : IImportFileService
{
    private static object _locker = new();

    public IXmlReaderService XmlReaderService { get; set; }
    public IEnergyConsumptionSaverService EnergyConsumptionSaverService { get; set; }

    public void Import(List<UploadedFile> uploadedFiles, bool overrideExisting)
    {
        try
        {
            if (!UploadTaskHelper.IsFinished)
                throw new WorkflowException("Выполняется другой процесс . Дождитесь окончания или отмените его.");

            lock (_locker)
            {
                if (!UploadTaskHelper.IsFinished)
                    throw new WorkflowException("Выполняется другой процесс . Дождитесь окончания или отмените его.");

                UploadTaskHelper.Init();
            }

            var energyComsumption = XmlReaderService.ParseXml(uploadedFiles);

            EnergyConsumptionSaverService.SaveToDatabase(energyComsumption, overrideExisting);

            UploadTaskHelper.SetToFinished();
        }
        catch (WorkflowException ex)
        {
            UploadTaskHelper.SetToFinishedWithError(ex.Message);
        }
        catch (Exception ex)
        {
            UploadTaskHelper.SetToFinishedWithError($"Ошибка при импорте данных: {ex}");
        }
    }
}
