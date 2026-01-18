using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<User> Users => Set<User>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(u => u.PasswordHash)
                    .IsRequired();

                entity.Property(u => u.Role)
                    .IsRequired();
            });

            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(t => t.Description)
                    .HasMaxLength(1000);

                entity.HasOne(t => t.User)
                    .WithMany()
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(rt => rt.Token)
                    .IsRequired();

                entity.Property(rt => rt.DeviceId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(rt => rt.ExpiresAt)
                    .IsRequired();

                entity.HasIndex(rt => new { rt.UserId, rt.DeviceId });

                entity.HasOne(rt => rt.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
