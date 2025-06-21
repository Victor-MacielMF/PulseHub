using Microsoft.EntityFrameworkCore;
using PulseHub.Domain.Entities;

namespace PulseHub.Infrastructure.Data
{
    public class PulseHubDbContext : DbContext
    {
        public PulseHubDbContext(DbContextOptions<PulseHubDbContext> options)
            : base(options)
        {
        }

        // DbSets → Tabelas
        public DbSet<Product> Products { get; set; }
        public DbSet<SyncEvent> SyncEvents { get; set; }
        public DbSet<QueueMessage> QueueMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔗 Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.Description)
                      .HasMaxLength(500);

                entity.Property(e => e.Price)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Stock)
                      .HasDefaultValue(0);

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true);

                entity.HasMany(e => e.SyncEvents)
                      .WithOne(e => e.Product)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 🔗 SyncEvent
            modelBuilder.Entity<SyncEvent>(entity =>
            {
                entity.HasKey(e => e.SyncEventId);

                entity.Property(e => e.EventType)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.Status)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.EventDate)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.Message)
                      .HasMaxLength(1000);

                entity.HasMany(e => e.QueueMessages)
                      .WithOne(e => e.SyncEvent)
                      .HasForeignKey(e => e.SyncEventId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 🔗 QueueMessage
            modelBuilder.Entity<QueueMessage>(entity =>
            {
                entity.HasKey(e => e.QueueMessageId);

                entity.Property(e => e.Payload)
                      .IsRequired();

                entity.Property(e => e.Channel)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.PublishedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.IsProcessed)
                      .HasDefaultValue(false);
            });
        }
    }
}
