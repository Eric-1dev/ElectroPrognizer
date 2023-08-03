namespace ElectroPrognizer.Services.Models.Prognizer;

public class DayData
{
    public DateTime Date { get; set; }

    public HourData[] HourDatas { get; set; }
}
