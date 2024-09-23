using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web253502Nikolaychik.Domain.Entities;

namespace Web253502Nikolaychik.API.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Commodity> Commodities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Настройка каскадного удаления
            modelBuilder.Entity<Commodity>()
                .HasOne(c => c.Category)
                .WithMany(cat => cat.Commodities)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Cascade); // Указывает на каскадное удаление
        }

    }
}
