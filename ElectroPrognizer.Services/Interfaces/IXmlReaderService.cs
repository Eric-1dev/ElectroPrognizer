using ElectroPrognizer.DataModel.Entities;

namespace ElectroPrognizer.Services.Interfaces;

public interface IXmlReaderService
{
    public List<EnergyConsumption> ParseXml(params string[] fileNames);
}
