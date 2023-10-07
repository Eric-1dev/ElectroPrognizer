namespace ElectroPrognizer.Services.Models.Prognizer;

public class PrevPrognozeHourData
{
    public int Hour { get; set; }

    public double? RealValue { get; set; }

    public double? PrognozedValue { get; set; }

    public double? ErrorPercent
    {
        get
        {
            if (!RealValue.HasValue || RealValue == 0)
                return null;

            return (PrognozedValue - RealValue) * 100 / RealValue;
        }
    }
}
