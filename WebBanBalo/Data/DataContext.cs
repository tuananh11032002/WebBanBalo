using Microsoft.EntityFrameworkCore;
using WebBanBalo.Model;

namespace WebBanBalo.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<Product>  Product { get; set; }

        public DbSet<Users> Users { get; set; }
        public DbSet<ColorProduct> ColorProducts { get; set; }
        public DbSet<Color> Color { get; set; }
        public DbSet<ProductImage> ProductImage { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Categories)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
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
            modelBuilder.Entity<Notification>().HasOne(p => p.User).WithMany(p => p.Notification).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);

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


            modelBuilder.Entity<ProductImage>().HasOne(p => p.Product).WithMany(p => p.Images).HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<Product>()
                .Property(p => p.CreatedAt)
                .HasDefaultValue(DateTime.Now);
        }
    }
}
