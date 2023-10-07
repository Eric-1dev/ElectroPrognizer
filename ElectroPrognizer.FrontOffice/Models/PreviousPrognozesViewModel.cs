using System.ComponentModel;

namespace ElectroPrognizer.FrontOffice.Models;

public class PreviousPrognozesViewModel
{
    [DisplayName("Год")]
    public int[] AvailableYears { get; set; }

    [DisplayName("Месяц")]
    public Dictionary<int, string> AvailableMonths { get; }

    public int CurrentYear { get; set; }

    public int CurrentMonth { get; set; }

    [DisplayName("Подстанция")]
    public SubstationViewModel[] Substations { get; set; }

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
