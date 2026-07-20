using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();

        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Payment> Payments => Set<Payment>();

        public DbSet<Review> Reviews => Set<Review>();

        public DbSet<Discount> Discounts => Set<Discount>();
        public DbSet<PromoCode> PromoCodes => Set<PromoCode>();

        public DbSet<NewsletterSubscriber> NewsletterSubscribers => Set<NewsletterSubscriber>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasOne(u => u.Cart)
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.UserId);


            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            builder.Entity<Product>()
                .HasIndex(p => p.SKU)
                .IsUnique();


            builder.Entity<Cart>()
                .HasIndex(c => c.UserId)
                .IsUnique();


            builder.Entity<Order>()
                .Property(o => o.Subtotal)
                .HasPrecision(18, 2);

            builder.Entity<Order>()
                .Property(o => o.DiscountAmount)
                .HasPrecision(18, 2);

            builder.Entity<Order>()
                .Property(o => o.Total)
                .HasPrecision(18, 2);


            builder.Entity<OrderItem>()
                .Property(o => o.UnitPrice)
                .HasPrecision(18, 2);


            builder.Entity<CartItem>()
                .Property(c => c.UnitPrice)
                .HasPrecision(18, 2);


            builder.Entity<Discount>()
                .Property(d => d.Value)
                .HasPrecision(18, 2);


            builder.Entity<PromoCode>()
                .HasIndex(p => p.Code)
                .IsUnique();

            builder.Entity<PromoCode>()
                .Property(p => p.Value)
                .HasPrecision(18, 2);

            builder.Entity<PromoCode>()
                .Property(p => p.MinimumOrder)
                .HasPrecision(18, 2);


            builder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId);

            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);


            builder.Entity<Review>()
                .HasIndex(r => new { r.UserId, r.ProductId })
                .IsUnique();


            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}