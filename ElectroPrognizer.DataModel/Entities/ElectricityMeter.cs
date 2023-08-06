using System.ComponentModel.DataAnnotations.Schema;

namespace ElectroPrognizer.DataModel.Entities;

public class ElectricityMeter : IdentityEntity
{
    public string Name { get; set; }
    
    public string Description { get; set; }

    public int SubstationId { get; set; }

    [ForeignKey(nameof(SubstationId))]
    public Substation Substation { get; set; }

    public virtual ICollection<EnergyConsumption> EnergyConsumptions { get; set; }

    public bool IsPositiveCounter { get; set; }
}
