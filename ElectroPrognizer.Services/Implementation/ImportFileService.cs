using ElectroPrognizer.Entities.Exceptions;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models;

namespace ElectroPrognizer.Services.Implementation;

public class ImportFileService : IImportFileService
{
    private static object _locker = new();

    public IXmlReaderService XmlReaderService { get; set; }
    public IEnergyConsumptionSaverService EnergyConsumptionSaverService { get; set; }
    public IUploadService UploadService { get; set; }

    public void Import(IEnumerable<FileData> uploadedFiles, bool overrideExisting)
    {
        try
        {
            ThrowIfBusy();

            lock (_locker)
            {
                ThrowIfBusy();

                UploadService.Init();
            }

            var energyComsumption = XmlReaderService.ParseXml(uploadedFiles);

            EnergyConsumptionSaverService.SaveToDatabase(energyComsumption, overrideExisting);

            UploadService.SetToFinished();
        }
        catch (UploaderIsBusyException ex)
        {
            UploadService.SendStatus(ex.Message);
            throw;
        }
        catch (WorkflowException ex)
        {
            UploadService.SetToFinishedWithError(ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            UploadService.SetToFinishedWithError($"Ошибка при импорте данных: {ex}");
            throw;
        }
    }
    
    private void ThrowIfBusy()
    {
        if (!UploadService.IsFinished)
            throw new UploaderIsBusyException("Выполняется другой процесс . Дождитесь окончания или отмените его.");
    }
}
