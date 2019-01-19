using Microsoft.EntityFrameworkCore;

namespace Pexip.Monitoring.Web.Data
{
    public class SQLiteContext : DbContext
    {
        public SQLiteContext(DbContextOptions<SQLiteContext> options) : base(options) { }

        public DbSet<ParticipantHistoryModel> ParticipantHistory { get; set; }
        public DbSet<ConferenceHistoryModel> ConferenceHistory { get; set; }
        public DbSet<MediaStreamHistoryModel> MediaStreamHistory { get; set; }

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

            // Use the ParticipantId and the Stream's Id to make a composite PK
            modelBuilder.Entity<MediaStreamHistoryModel>()
                .HasKey(i => new { i.ParticipantId, i.Id });

        }
    }
}
