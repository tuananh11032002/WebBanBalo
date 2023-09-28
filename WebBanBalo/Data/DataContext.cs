using Microsoft.EntityFrameworkCore;
using WebBanBalo.Model;

namespace WebBanBalo.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Message> Message { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<Product>  Product { get; set; }
        public DbSet<ProductCategory> ProductCategory { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<ColorProduct> ColorProducts { get; set; }
        public DbSet<Color> Color { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
             
            modelBuilder.Entity<ProductCategory>()
              .HasKey(p => new { p.CategoryId, p.ProductId });
            modelBuilder.Entity<ProductCategory>()
                .HasOne(p => p.Category)
                .WithMany(pc=>pc.ProductCategories)
                .HasForeignKey(p => p.CategoryId);
            modelBuilder.Entity<ProductCategory>()
                .HasOne(p => p.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(p => p.ProductId);


            modelBuilder.Entity<OrderItem>()
                .HasKey(p => new { p.ProductId, p.OrderId });
            modelBuilder.Entity<OrderItem>()
                .HasOne(p => p.Order)
                .WithMany(pc => pc.OrderItems)
                .HasForeignKey(fk => fk.OrderId);
            modelBuilder.Entity<OrderItem>().
                HasOne(p => p.Product).
                WithMany(p=>p.OrderItems).
                HasForeignKey(p=>p.ProductId);

            modelBuilder.Entity<Order>().Property(p => p.Done).HasDefaultValue(false);
            modelBuilder.Entity<Order>().Property(p => p.TotalAmount).HasDefaultValue(0);


            modelBuilder.Entity<ColorProduct>().HasKey(p => new { p.ProductId, p.ColorId });
            modelBuilder.Entity<ColorProduct>()
                .HasOne(cp => cp.Color)
                .WithMany(c => c.Products)
                .HasForeignKey(cp => cp.ColorId);
            modelBuilder.Entity<ColorProduct>()
                .HasOne(p => p.Product)
                .WithMany(pr => pr.Colors)
                .HasForeignKey(p => p.ProductId);
            modelBuilder.Entity<Product>().Property(p=>p.Soluong).HasDefaultValue(0);

            modelBuilder.Entity<Users>().Property(p => p.Role).HasDefaultValue("user");

            modelBuilder.Entity<Message>()
                .HasOne(m => m.ReceiveUser)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverUserId)
                        .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Message>()
                .HasOne(m => m.SenderUser)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderUserId)           
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
