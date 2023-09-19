using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ElectroPrognizer.FrontOffice.Models;

public class SubstationViewModel
{
    [DisplayName("Идентификатор")]
    [ReadOnly(isReadOnly: true)]
    public int Id { get; set; }

    [DisplayName("ИНН")]
    [ReadOnly(isReadOnly: true)]
    public string Inn { get; set; }

    [DisplayName("Название")]
    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    public string Name { get; set; }

    [DisplayName("Описание")]
    public string Description { get; set; }

    [DisplayName("Список счетчиков")]
    public ElectricityMeterViewModel[] ElectricityMeters { get; set; }

    [DisplayName("Константа, добавляемая к значению (в мегаваттах)")]
    public double AdditionalValueConstant { get; set; }

    [DisplayName("Широта")]
    public double? Latitude { get; set; }

    [DisplayName("Долгота")]
    public double? Longitude { get; set; }
}
