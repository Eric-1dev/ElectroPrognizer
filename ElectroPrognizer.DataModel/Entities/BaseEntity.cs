namespace ElectroPrognizer.DataModel.Entities
{
    public class BaseEntity : IdentityEntity
    {
        public DateTime Created { get; set; }

        public BaseEntity() => Created = DateTime.Now;
    }
}
