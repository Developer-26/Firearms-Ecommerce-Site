using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Gun_Site.Models;
using Gun_Site.Areas.Identity.Data;

namespace Gun_Site.Data
{
    public class AppDbContext1 : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext1(DbContextOptions<AppDbContext1> options) : base(options) { }

        public DbSet<Admin_Added_Products> AdminAddedProducts { get; set; }
        public DbSet<ProductSpecials> ProductSpecials { get; set; }
        public DbSet<AdministratorModel> Administrators { get; set; }
        public DbSet<CartModel> CartItems { get; set; } // Add this line for the CartModel

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Important for IdentityDbContext setup

            modelBuilder.Entity<Admin_Added_Products>()
                .ToTable("AdminAddedProducts")
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ProductSpecials>()
                .ToTable("ProductSpecials")
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // Optionally, configure CartModel (if needed)
            modelBuilder.Entity<CartModel>()
                .ToTable("CartItems") // Specify table name
                .Property(c => c.Price)
                .HasColumnType("decimal(18,2)"); // Ensure proper decimal handling
        }
    }
}