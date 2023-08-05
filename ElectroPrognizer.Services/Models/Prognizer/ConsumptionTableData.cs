namespace ElectroPrognizer.Services.Models.Prognizer;

public class ConsumptionTableData
{
    public DayData[] DayDatas { get; set; }

    public ConsumptionTableData()
    {
        DayDatas = Array.Empty<DayData>();
    }
}
