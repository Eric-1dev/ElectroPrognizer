using System.ComponentModel.DataAnnotations;

namespace ElectroPrognizer.FrontOffice.Models;

public class PrognizerViewModel
{
    [Display(Name = "Подстанция")]
    public ICollection<SubstationViewModel> Substations { get; set; }

    [Display(Name = "Прогноз на дату")]
    public string StartDate { get; set; }
}
