using System.ComponentModel;
using ElectroPrognizer.FrontOffice.Attributes;

namespace ElectroPrognizer.FrontOffice.Models;

public class DownloadLogViewModel
{
    public int? PageNumber { get; set; }

    [DisplayName("Дата с")]
    public DateTime? DateFrom { get; set; }

    [DisplayName("Дата по")]
    [DateGreaterThan(nameof(DateFrom))]
    public DateTime? DateTo { get; set; }
}
