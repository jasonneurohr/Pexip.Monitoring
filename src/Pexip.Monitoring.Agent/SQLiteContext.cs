using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pexip.Monitoring.Agent.Models;
using System.IO;
using System.Reflection;

namespace Pexip.Monitoring.Agent
{
    public class SQLiteContext : DbContext
    {
        public DbSet<ParticipantHistoryModel> ParticipantHistory { get; set; }
        public DbSet<ConferenceHistoryModel> ConferenceHistory { get; set; }
        public DbSet<MediaStreamHistoryModel> MediaStreamHistory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Uses the location of the currently running assembly as the base path,
            // to resolve the appsettings.json location.
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json", optional: true);

            IConfigurationRoot configuration = builder.Build();

            // Get the SQLite DB file location
            optionsBuilder.UseSqlite(@"Data Source=" + configuration.GetValue<string>("dbPath"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConferenceHistoryModel>()
                .HasKey(i => i.ConferenceId);

            modelBuilder.Entity<ParticipantHistoryModel>()
                .HasKey(i => i.ParticipantId);

            modelBuilder.Entity<ParticipantHistoryModel>()
                .Ignore(i => i.MediaStreams);

            modelBuilder.Entity<MediaStreamHistoryModel>()
                .HasOne(p => p.Participant)
                .WithMany(m => m.MediaStreams)
                .HasForeignKey(p => p.ParticipantId);

            // Use the ParticipantId  and the Stream's Id to make a composite PK
            modelBuilder.Entity<MediaStreamHistoryModel>()
                .HasKey(i => new { i.ParticipantId, i.Id });

            modelBuilder.Entity<ParticipantHistoryModel>()
                .HasIndex(i => new { i.StartTime, i.EndTime, i.CallQuality });
        }
    }
}
