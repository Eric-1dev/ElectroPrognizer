﻿using System.Text;
using System.Xml.Serialization;
using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.DataModel.Models.XmlModels;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models;
using ElectroPrognizer.Utils.Helpers;

namespace ElectroPrognizer.Services.Implementation;

public class XmlReaderService : IXmlReaderService
{
    public List<EnergyConsumption> ParseXml(string[] fileNames)
    {
        var files = new List<UploadedFile>();

        foreach (var fileName in fileNames)
        {
            var bytes = File.ReadAllBytes(fileName);

            files.Add(new UploadedFile { Name = fileName, Content = bytes });
        }

        return ParseXml(files);
    }

    public List<EnergyConsumption> ParseXml(IEnumerable<UploadedFile> files)
    {
        var messages = new List<Message>();

        var xmlSerializer = new XmlSerializer(typeof(Message));

        foreach (var file in files)
        {
            var utfEncodedBytes = Encoding.Convert(Encoding.GetEncoding(1251), Encoding.UTF8, file.Content);

            var xml = Encoding.UTF8.GetString(utfEncodedBytes);

            using TextReader tr = new StringReader(xml);

            var message = xmlSerializer.Deserialize(tr) as Message;

            messages.Add(message!);
        }

        var energyConsumptions = messages.SelectMany(x => x.MapToEnergyComsumption()).ToList();

        return energyConsumptions;
    }
}
