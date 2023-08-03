using System.ComponentModel.DataAnnotations;

namespace ElectroPrognizer.FrontOffice.Models;

public class PrognizerViewModel
{
    [Display(Name = "Месяц")]
    public IDictionary<int, string> Months { get; set; }

    [Display(Name = "Год")]
    public ICollection<int> Years { get; set; }

    [Display(Name = "Подстанция")]
    public ICollection<SubstationViewModel> Substation { get; set; }
}
