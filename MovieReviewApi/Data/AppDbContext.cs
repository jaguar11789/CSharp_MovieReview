using Microsoft.EntityFrameworkCore;
using MovieReviewApi.Models.Accounts.User;

namespace MovieReviewApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<UserHistoryEntity> UserHistories { get; set; }
        public DbSet<UserSocialAccountEntity> UserSocialAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserEntity>().ToTable("TUsers");
            modelBuilder.Entity<UserHistoryEntity>().ToTable("TUserHistory");
            modelBuilder.Entity<UserSocialAccountEntity>().ToTable("TUserSocialAccounts");
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
