using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ElectroPrognizer.FrontOffice.Models;

public class ElectricityMeterViewModel
{
    [DisplayName("Идентификатор счетчика")]
    public int Id { get; set; }

    [DisplayName("Идентификатор подстанции")]
    public int SubstationId { get; set; }

    [DisplayName("Название")]
    public string Name { get; set; }

    [DisplayName("Описание")]
    [Required(ErrorMessage = "Обязательно для заполнения")]
    public string Description { get; set; }

    [DisplayName("Показания учитываются в плюс")]
    public bool IsPositiveCounter { get; set; }
}
