using Microsoft.EntityFrameworkCore;
using FarmHelpAPI.Models;

namespace FarmHelpAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Trace> Traces { get; set; } = null!;
        public DbSet<TraceCode> TraceCodes { get; set; } = null!;
        public DbSet<Purchase> Purchases { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Unique indexes
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<TraceCode>().HasIndex(t => t.Code).IsUnique();

            // Product -> Farmer (User) relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Farmer)
                .WithMany()
                .HasForeignKey(p => p.FarmerId);

            // Review -> Product relationship
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany()
                .HasForeignKey(r => r.ProductId);

            // Review -> User relationship
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);

            // Trace -> Product relationship
            modelBuilder.Entity<Trace>()
                .HasOne(t => t.Product)
                .WithMany()
                .HasForeignKey(t => t.ProductId);

            // TraceCode -> Product relationship
            modelBuilder.Entity<TraceCode>()
                .HasOne(t => t.Product)
                .WithMany()
                .HasForeignKey(t => t.ProductId);

            // Purchase - no FK constraint on BuyerId (matching original design)
        }
    }
}
