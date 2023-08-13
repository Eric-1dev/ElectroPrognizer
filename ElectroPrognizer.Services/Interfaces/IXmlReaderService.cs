using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Services.Models;

namespace ElectroPrognizer.Services.Interfaces;

public interface IXmlReaderService
{
    List<EnergyConsumption> ParseXml(params string[] fileNames);

    List<EnergyConsumption> ParseXml(IEnumerable<FileData> files);
}
