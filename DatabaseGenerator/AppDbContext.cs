using MasCom.Lib;
using Microsoft.EntityFrameworkCore;

namespace DatabaseGenerator
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserToSessionsMapping> SessionMappings { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<FileMessageHeaders> FileMessageHeaders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema(null);

            builder.Entity<User>().HasData(new User()
            {
                Id = -1,
                LastName = "Eric",
                UserName = "Eric",
                PasswordHash = "ggg",
                Name = "Hotegni"
            },
            new User()
            {
                Id = -1200,
                LastName = "Manzourou",
                UserName = "Baba",
                PasswordHash = "1234",
                Name = "Manzourou Alao rrrr..Gbaaa"
            },
            new User()
            {
                Id = -220,
                LastName = "Samson",
                UserName = "Sam",
                PasswordHash = "1234",
                Name = "Oala Samson"
            });
        }
    }
}
