using System.Xml.Schema;
using System.Xml.Serialization;

namespace ElectroPrognizer.DataModel.Models.XmlModels;

[Serializable()]
[XmlRoot("messageSender")]
public class MessageSender
{
    [XmlElement("inn", Form = XmlSchemaForm.Unqualified)]
    public string Inn { get; set; }

    [XmlElement("name", Form = XmlSchemaForm.Unqualified)]
    public string Name { get; set; }
}
