using System.ComponentModel.DataAnnotations.Schema;
using ElectroPrognizer.Entities.Enums;

namespace ElectroPrognizer.DataModel.Entities
{
    public class MeasuringChannel : IdentityEntity
    {
        [Column("MeasuringChannelCode")]
        public MeasuringChannelTypeEnum MeasuringChannelType { get; set; }

        public string Description { get; set; }
    }
}
