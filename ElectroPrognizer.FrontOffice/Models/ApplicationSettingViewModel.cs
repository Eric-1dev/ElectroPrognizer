using System.ComponentModel.DataAnnotations;

namespace ElectroPrognizer.FrontOffice.Models;

public class ApplicationSettingViewModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string InternalName { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public string Value { get; set; }
}
