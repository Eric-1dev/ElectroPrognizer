namespace ElectroPrognizer.Services.Models.Prognizer;

public class ConsumptionTableData
{
    public int Month { get; set; }

    public int Year { get; set; }

    public DayData[] DayDatas { get; set; }

    public ConsumptionTableData()
    {
        DayDatas = Array.Empty<DayData>();
    }
}
