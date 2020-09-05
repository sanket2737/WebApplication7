using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication7.Models;

namespace WebApplication7.Data
{
    // IdentityDbContext contains all the user tables
    public class ExtendedAppDbContext : IdentityDbContext<ExtendedIdentityUserModel>
    {
        public ExtendedAppDbContext(DbContextOptions<ExtendedAppDbContext> options)
            : base(options)
        {

        }

        public DbSet<ExtendedIdentityUserModel> extendedIdentityUserModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Seed();
        }
    }
}
