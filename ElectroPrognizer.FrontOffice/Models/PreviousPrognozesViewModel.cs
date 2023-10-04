using ElectroPrognizer.Services.Models.Prognizer;

namespace ElectroPrognizer.FrontOffice.Models;

public class PreviousPrognozesViewModel
{
    public int[] AvailableYears { get; set; }

    public Dictionary<int, string> AvailableMonths { get; }

    public int CurrentYear { get; set; }

    public int CurrentMonth { get; set; }

    public PreviousPrognozesViewModel()
    {
        AvailableMonths = new Dictionary<int, string>
        {
            { 1, "Январь" },
            { 2, "Февраль" },
            { 3, "Март" },
            { 4, "Апрель" },
            { 5, "Май" },
            { 6, "Июнь" },
            { 7, "Июль" },
            { 8, "Август" },
            { 9, "Сентябрь" },
            { 10, "Октябрь" },
            { 11, "Ноябрь" },
            { 12, "Декабрь" }
        };
    }
}
