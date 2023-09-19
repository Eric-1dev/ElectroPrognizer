namespace ElectroPrognizer.Services.Dto;

public class SubstationDto
{
    public int Id { get; set; }
    public string Inn { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double AdditionalValueConstant { get; set; }
    public double? Latitude {  get; set; }
    public double? Longitude { get; set; }
}
