using System.Xml.Schema;
using System.Xml.Serialization;

namespace ElectroPrognizer.DataModel.Models.XmlModels;

[Serializable()]
[XmlRoot("messageAreaMeasuringpointMeasuringchannelPeriod")]
public class MessageAreaMeasuringpointMeasuringchannelPeriod
{
    [XmlElement("value", Form = XmlSchemaForm.Unqualified)]
    public string Value { get; set; }

    [XmlAttribute("start")]
    public string Start { get; set; }

    [XmlAttribute("end")]
    public string End { get; set; }
}