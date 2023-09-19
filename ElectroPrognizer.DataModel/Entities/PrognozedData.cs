namespace ElectroPrognizer.DataModel.Entities;

public class PrognozedData : IdentityEntity
{
    public int SubstationId { get; set; }

    public DateTime PrognozeDate { get; set; }

    public double Value { get; set; }
}
