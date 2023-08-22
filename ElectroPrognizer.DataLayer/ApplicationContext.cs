using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Utils.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ElectroPrognizer.DataLayer
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Substation> Substations { get; set; }
        public DbSet<EnergyConsumption> EnergyConsumptions { get; set; }
        public DbSet<ElectricityMeter> ElectricityMeters { get; set; }
        public DbSet<MeasuringChannel> MeasuringChannels { get; set; }
        public DbSet<ApplicationSetting> ApplicationSettings { get; set; }
        public DbSet<DownloadLogEntity> DownloadLogs { get; set; }

        public ApplicationContext()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EnergyConsumption>().HasIndex(x => new { x.StartDate, x.ElectricityMeterId, x.MeasuringChannelId }, "IX_Date_NodeId_MeasuringChannelId");
            modelBuilder.Entity<EnergyConsumption>().HasIndex(x => new { x.StartDate }, "IX_StartDate");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConfigurationHelper.ConntectionString);

#if DEBUG
            //optionsBuilder.LogTo((message) => { Debug.WriteLine(message); });
#endif
        }
    }
}
