using System.ComponentModel.DataAnnotations;

namespace ElectroPrognizer.DataModel.Entities;

public class Substation : IdentityEntity
{
    [StringLength(256)]
    [Required]
    public string Inn { get; set; }

    [StringLength(1024)]
    public string Name { get; set; }

    [StringLength(1024)]
    public string Description { get; set; }

    public virtual ICollection<ElectricityMeter> ElectricityMeters { get; set; }

    public double AdditionalValueConstant { get; set; }
}
