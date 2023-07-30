using System.Xml.Schema;
using System.Xml.Serialization;

namespace ElectroPrognizer.DataModel.Models.XmlModels;

[Serializable()]
[XmlRoot("messageDatetime")]
public class MessageDatetime
{
    [XmlElement("timestamp", Form = XmlSchemaForm.Unqualified)]
    public string Timestamp { get; set; }

    [XmlElement("daylightsavingtime", Form = XmlSchemaForm.Unqualified)]
    public string DaylightSavingTime { get; set; }

    [XmlElement("day", Form = XmlSchemaForm.Unqualified)]
    public string Day { get; set; }
}
