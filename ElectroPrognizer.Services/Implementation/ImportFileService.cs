using ElectroPrognizer.Entities.Models;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models;

namespace ElectroPrognizer.Services.Implementation;

public class ImportFileService : IImportFileService
{
    public IXmlReaderService XmlReaderService { get; set; }
    public IEnergyConsumptionSaverService EnergyConsumptionSaverService { get; set; }

    public OperationResult Import(List<UploadedFile> uploadedFIles, bool overrideExisting)
    {
        try
        {
            var energyComsumption = XmlReaderService.ParseXml(uploadedFIles);

            EnergyConsumptionSaverService.SaveToDatabase(energyComsumption, overrideExisting);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }
}
