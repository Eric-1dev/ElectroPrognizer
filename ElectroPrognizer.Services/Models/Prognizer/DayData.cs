namespace ElectroPrognizer.Services.Models.Prognizer;

public class DayData
{
    public DateTime Date { get; set; }

    public bool IsRealData { get; set; }

    public double Total { get; set; }

    public double CumulativeTotal { get; set; }

    public HourData[] HourDatas { get; set; }
}
