namespace ElectroPrognizer.Services.Dto;

public class ElectricityMeterDto
{
    public int Id { get; set; }

    public int SubstationId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsPositiveCounter { get; set; }
}
