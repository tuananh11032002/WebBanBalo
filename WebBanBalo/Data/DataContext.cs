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
        public DbSet<OrderStatusUpdate> OrderStatusUpdates { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<ProductImage> ProductImage { get; set; }

        public DbSet<Review> Review { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Payment>()
                       .HasOne(p => p.Order)  
                       .WithMany(o => o.Payment)  
                       .HasForeignKey(p => p.OrderId);
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Categories)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<OrderStatusUpdate>().HasOne(p => p.Order)
                .WithMany(p => p.OrderStatusUpdates).HasForeignKey(p => p.OrderId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Product>().Property(p => p.Discount).HasDefaultValue(0);
            modelBuilder.Entity<Product>().Property(p => p.Status).HasDefaultValue(StatusProduct.Publish);

            modelBuilder.Entity<Product>().Property(p => p.Soluong).HasDefaultValue(0);

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


            modelBuilder.Entity<Users>().HasMany(p => p.ReceivedMessages).WithOne(p => p.ReceiveUser).HasForeignKey(px => px.ReceiverUserId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Users>().HasMany(p => p.SentMessages).WithOne(p => p.SenderUser).HasForeignKey(px => px.SenderUserId).OnDelete(DeleteBehavior.NoAction);



            modelBuilder.Entity<ProductImage>().HasOne(p => p.Product).WithMany(p => p.Images).HasForeignKey(p => p.ProductId);

            
            modelBuilder.Entity<Review>().HasOne(p=>p.Product).WithMany(p=>p.Reviews).HasForeignKey(p=> p.ProductId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Review>().HasOne(p=>p.User).WithMany(p=>p.Reviews).HasForeignKey(p=>p.UserId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
