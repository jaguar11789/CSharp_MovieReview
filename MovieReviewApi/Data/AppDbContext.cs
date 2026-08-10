using Microsoft.EntityFrameworkCore;
using MovieReviewApi.Models.Accounts.User;

namespace MovieReviewApi.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<UserHistoryEntity> UserHistories { get; set; }
        public DbSet<UserSocialAccountEntity> UserSocialAccounts { get; set; }
        public DbSet<EmailVerificationEntity> EmailVerifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserEntity>().ToTable("TUsers");
            modelBuilder.Entity<UserHistoryEntity>().ToTable("TUserHistory");
            modelBuilder.Entity<UserSocialAccountEntity>().ToTable("TUserSocialAccounts");
            modelBuilder.Entity<EmailVerificationEntity>(entity =>
            {
                entity.ToTable("TEmailVerifications");
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.User)
                      .WithMany()
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserHistoryEntity>()
                .HasOne(h => h.User)
                .WithMany(u => u.UserHistories)
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserSocialAccountEntity>()
                .HasOne(x => x.User)
                .WithMany(u => u.UserSocialAccounts)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
