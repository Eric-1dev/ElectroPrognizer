using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ElectroPrognizer.Entities.Enums;

namespace ElectroPrognizer.DataModel.Entities;

public class MeasuringChannel : IdentityEntity
{
    [Column("MeasuringChannelCode")]
    [Required]
    public MeasuringChannelTypeEnum MeasuringChannelType { get; set; }

    [StringLength(1024)]
    public string Description { get; set; }
}
