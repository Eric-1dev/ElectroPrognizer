using System.ComponentModel.DataAnnotations;
using ElectroPrognizer.Entities.Enums;

namespace ElectroPrognizer.DataModel.Entities;

public class DownloadLogEntity : BaseEntity
{
    [Required]
    public LogLevelEnum LogLevel { get; set; }

    [StringLength(8000)]
    [Required]
    public string Message { get; set; }
}
