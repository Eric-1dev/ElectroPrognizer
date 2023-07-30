using System.ComponentModel.DataAnnotations.Schema;

namespace ElectroPrognizer.DataModel.Entities
{
    public class EnergyConsumption : IdentityEntity
    {
        public DateTime Date { get; set; }
        public int NodeId { get; set; }

        [ForeignKey(nameof(NodeId))]
        public Node Node { get; set; }

        public double Value { get; set; }

        public int MeasuringChannelId { get; set; }

        [ForeignKey(nameof(MeasuringChannelId))]
        public MeasuringChannel MeasuringChannel { get; set; }
    }
}
