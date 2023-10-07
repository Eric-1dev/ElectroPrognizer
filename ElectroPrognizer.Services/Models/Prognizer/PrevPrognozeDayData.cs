namespace ElectroPrognizer.Services.Models.Prognizer;

public class PrevPrognozeDayData
{
    public DateTime Date { get; set; }

    public List<PrevPrognozeHourData> Hours { get; set; }

    public PrevPrognozeDayData(DateTime date)
    {
        Date = date.Date;
        Hours = new List<PrevPrognozeHourData>();
    }
}
