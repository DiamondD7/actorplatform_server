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


            //User <=> UserBackground (one to one relationship)
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserBackground)
                .WithOne(p => p.User)
                .HasForeignKey<UserBackground>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
