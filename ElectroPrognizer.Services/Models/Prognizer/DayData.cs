namespace ElectroPrognizer.Services.Models.Prognizer;

public class DayData
{
    public int DayNumber { get; set; }

    public double Total { get; set; }

    public double CumulativeTotal { get; set; }

    public HourData[] HourDatas { get; set; }
}
