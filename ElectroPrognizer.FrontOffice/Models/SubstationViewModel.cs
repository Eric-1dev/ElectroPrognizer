using System.ComponentModel;

namespace ElectroPrognizer.FrontOffice.Models;

public class SubstationViewModel
{
    [DisplayName("Идентификатор")]
    public int Id { get; set; }

    [DisplayName("ИНН")]
    public string Inn { get; set; }

    [DisplayName("Название")]
    public string Name { get; set; }

    [DisplayName("Описание")]
    public string Description { get; set; }
}
