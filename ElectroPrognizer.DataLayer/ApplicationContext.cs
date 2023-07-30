using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Utils.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ElectroPrognizer.DataLayer
{
    public class ApplicationContext : DbContext
    {
        public DbSet<EnergyConsumption> EnergyConsumptions { get; set; }
        public DbSet<Node> Nodes { get; set; }
        public DbSet<MeasuringChannel> MeasuringChannels { get; set; }

        public ApplicationContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EnergyConsumption>().HasIndex(x => new { x.Date, x.NodeId, x.MeasuringChannelId }, "IX_Date_NodeId_MeasuringChannelId");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConfigurationHelper.ConntectionString);
        }
    }
}