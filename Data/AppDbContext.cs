using Microsoft.EntityFrameworkCore;
using UserAPI.Models;

namespace UserAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext>options) : base(options)
        {
                
        }

        public DbSet<User> UsersTable {  get; set; }
        public DbSet<UserBackground>UserBackground { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            //User <=> UserBackground (one to many relationship)
            modelBuilder.Entity<User>()
                .HasMany(u => u.UserBackgrounds)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //UserBackground <=> VideoUrls (one to many relationship)
            modelBuilder.Entity<UserBackground>()
                .HasMany(u => u.VideoUrls)
                .WithOne(v => v.UserBackground)
                .HasForeignKey(v => v.UserBackgroundId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }

}
