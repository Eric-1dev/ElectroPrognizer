using System.Xml.Schema;
using System.Xml.Serialization;

namespace ElectroPrognizer.DataModel.Models.XmlModels;

[Serializable()]
[XmlRoot("messageArea")]
public class MessageArea
{
    [XmlElement("inn", Form = XmlSchemaForm.Unqualified)]
    public string Inn { get; set; }

    [XmlElement("name", Form = XmlSchemaForm.Unqualified)]
    public string Name { get; set; }

    [XmlElement("measuringpoint", Form = XmlSchemaForm.Unqualified)]
    public MessageAreaMeasuringpoint[] MeasuringPoint { get; set; }
}
