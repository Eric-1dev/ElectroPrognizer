using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectroPrognizer.DataModel.Entities;

public class ElectricityMeter : IdentityEntity
{
    [StringLength(1024)]
    public string Name { get; set; }

    [StringLength(1024)]
    public string Description { get; set; }

    [Required]
    public int SubstationId { get; set; }

    [ForeignKey(nameof(SubstationId))]
    public Substation Substation { get; set; }

    public virtual ICollection<EnergyConsumption> EnergyConsumptions { get; set; }

    public bool IsPositiveCounter { get; set; }
}
