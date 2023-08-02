namespace ElectroPrognizer.FrontOffice.Models;

public class PrognizerViewModel
{
    public IDictionary<int, string> Months { get; set; }

    public ICollection<int> AvailableYears { get; set; }
}
