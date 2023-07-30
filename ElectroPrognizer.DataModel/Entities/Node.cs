using System.ComponentModel.DataAnnotations;

namespace ElectroPrognizer.DataModel.Entities;

public class Node : IdentityEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public virtual List<EnergyConsumption> EnergyConsumptions { get; set; }
}
