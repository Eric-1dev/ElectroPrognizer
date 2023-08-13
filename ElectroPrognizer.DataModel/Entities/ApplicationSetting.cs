using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ElectroPrognizer.Entities.Enums;

namespace ElectroPrognizer.DataModel.Entities;

public class ApplicationSetting
{
    [Key]
    [Column("Id")]
    [Required]
    public ApplicationSettingEnum ApplicationSettingType { get; set; }

    [StringLength(256)]
    [Required]
    public string InternalName { get; set; }

    [StringLength(1024)]
    [Required]
    public string Description { get; set; }

    [StringLength(4096)]
    public string Value { get; set; }
}
