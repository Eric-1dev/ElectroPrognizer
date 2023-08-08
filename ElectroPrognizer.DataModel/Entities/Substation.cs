namespace ElectroPrognizer.DataModel.Entities;

public class Substation : IdentityEntity
{
    public string Inn { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public virtual ICollection<ElectricityMeter> ElectricityMeters { get; set; }

    public double AdditionalValueConstant { get; set; }
}
