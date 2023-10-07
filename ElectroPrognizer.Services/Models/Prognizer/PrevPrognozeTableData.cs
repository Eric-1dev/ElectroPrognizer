using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ElectroPrognizer.Services.Models.Prognizer;

public class PrevPrognozeTableData
{
    public List<PrevPrognozeDayData> Days { get; set; }

    public PrevPrognozeTableData()
    {
        Days = new List<PrevPrognozeDayData>();
    }
}
