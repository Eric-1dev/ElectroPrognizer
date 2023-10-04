using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectroPrognizer.DataModel.Entities;

public class EnergyConsumption : IdentityEntity
{
    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public int ElectricityMeterId { get; set; }

    [ForeignKey(nameof(ElectricityMeterId))]
    public ElectricityMeter ElectricityMeter { get; set; }

    [Required]
    public double Value { get; set; }

    [Required]
    public int MeasuringChannelId { get; set; }

    [ForeignKey(nameof(MeasuringChannelId))]
    public MeasuringChannel MeasuringChannel { get; set; }
}
