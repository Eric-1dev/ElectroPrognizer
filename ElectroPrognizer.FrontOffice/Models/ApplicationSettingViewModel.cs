using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ElectroPrognizer.FrontOffice.Models;

public class ApplicationSettingViewModel
{
    [Required]
    [DisplayName("Идентификатор")]
    public int Id { get; set; }

    [Required]
    [DisplayName("Внутреннее наименование")]
    public string InternalName { get; set; }

    [Required]
    [DisplayName("Описание")]
    public string Description { get; set; }

    [Required]
    [DisplayName("Значение настройки")]
    public string Value { get; set; }
}
