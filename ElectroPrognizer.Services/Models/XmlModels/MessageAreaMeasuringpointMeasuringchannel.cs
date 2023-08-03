using System.Xml.Schema;
using System.Xml.Serialization;

namespace ElectroPrognizer.Services.Models.XmlModels;

[Serializable()]
[XmlRoot("messageAreaMeasuringpointMeasuringchannel")]
public class MessageAreaMeasuringpointMeasuringchannel
{
    [XmlElement("period", Form = XmlSchemaForm.Unqualified)]
    public MessageAreaMeasuringpointMeasuringchannelPeriod[] Period { get; set; }

    [XmlAttribute("code")]
    public string Code { get; set; }

    [XmlAttribute("desc")]
    public string Desc { get; set; }
}
