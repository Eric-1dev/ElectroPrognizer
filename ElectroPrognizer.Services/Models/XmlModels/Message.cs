using System.Xml.Schema;
using System.Xml.Serialization;

namespace ElectroPrognizer.Services.Models.XmlModels;

[Serializable()]
[XmlRoot(Namespace = "", ElementName = "message", IsNullable = false)]
public class Message
{
    [XmlElement("datetime", Form = XmlSchemaForm.Unqualified)]
    public MessageDatetime Datetime { get; set; }

    [XmlElement("sender", Form = XmlSchemaForm.Unqualified)]
    public MessageSender Sender { get; set; }

    [XmlElement("area", Form = XmlSchemaForm.Unqualified)]
    public MessageArea[] Area { get; set; }

    /// <remarks/>
    [XmlAttribute("class")]
    public string Class { get; set; }

    /// <remarks/>
    [XmlAttribute("version")]
    public string Version { get; set; }

    /// <remarks/>
    [XmlAttribute("number")]
    public string Number { get; set; }
}
