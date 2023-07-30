using System.Xml.Schema;
using System.Xml.Serialization;

namespace ElectroPrognizer.DataModel.Models.XmlModels;

[Serializable()]
[XmlRoot("messageAreaMeasuringpoint")]
public class MessageAreaMeasuringpoint
{
    [XmlElement("measuringchannel", Form = XmlSchemaForm.Unqualified)]
    public MessageAreaMeasuringpointMeasuringchannel[] MeasuringChannel { get; set; }

    [XmlAttribute("code")]
    public string Code { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }
}
