using System.ComponentModel.DataAnnotations;

namespace ElectroPrognizer.FrontOffice.Models;

public class PrognizerViewModel
{
    [Display(Name = "Прогноз на дату")]
    public string StartDate { get; set; }

    [Display(Name = "Подстанция")]
    public ICollection<SubstationViewModel> Substation { get; set; }
}
