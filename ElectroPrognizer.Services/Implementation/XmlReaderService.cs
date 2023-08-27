using System.Text;
using System.Xml.Serialization;
using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Services.Extensions;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models;
using ElectroPrognizer.Services.Models.XmlModels;

namespace ElectroPrognizer.Services.Implementation;

public class XmlReaderService : IXmlReaderService
{
    public List<EnergyConsumption> ParseXml(string[] fileNames)
    {
        var files = new List<FileData>();

        foreach (var fileName in fileNames)
        {
            var bytes = File.ReadAllBytes(fileName);

            files.Add(new FileData(fileName, bytes));
        }

        return ParseXml(files);
    }

    public List<EnergyConsumption> ParseXml(IEnumerable<FileData> files)
    {
        var messages = new List<Message>();

        var xmlSerializer = new XmlSerializer(typeof(Message));

        foreach (var file in files)
        {
            var utfEncodedBytes = Encoding.Convert(Encoding.GetEncoding(1251), Encoding.UTF8, file.Content);

            var xml = Encoding.UTF8.GetString(utfEncodedBytes);

            using var stringReader = new StringReader(xml);

            var message = xmlSerializer.Deserialize(stringReader) as Message;

            messages.Add(message!);
        }

        var energyConsumptions = messages.SelectMany(x => x.MapToEnergyComsumption()).ToList();

        return energyConsumptions;
    }
}
