using System.ComponentModel.DataAnnotations;

namespace ElectroPrognizer.DataModel.Entities;

public class BaseEntity : IdentityEntity
{
    [Required]
    public DateTime Created { get; set; }

    public BaseEntity() => Created = DateTime.Now;
}
