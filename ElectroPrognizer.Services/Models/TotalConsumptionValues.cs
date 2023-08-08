namespace ElectroPrognizer.Services.Models;

public class TotalConsumptionValues
{
    public double? TotalForDay { get; }
    public double? CumulativeTotalForMonth { get; }

    public TotalConsumptionValues(double? totalForDay, double? cumulativeTotalForMonth)
    {
        TotalForDay = totalForDay;
        CumulativeTotalForMonth = cumulativeTotalForMonth;
    }
}
