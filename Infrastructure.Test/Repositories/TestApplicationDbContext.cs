using Domain.Common;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class TestApplicationDbContext : ApplicationDbContext
{
    public TestApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<TestProduct> TestProducts => Set<TestProduct>();
    public DbSet<TestCategory> TestCategories => Set<TestCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestProduct>(builder => {
            builder.HasKey(p => p.Id);
            builder.HasOne(p => p.Category)
                   .WithMany()
                   .IsRequired(false);
        });

        modelBuilder.Entity<TestCategory>(builder => {
            builder.HasKey(c => c.Id);
        });
    }
    public class TestProduct : Entity
    {
        public string Name { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public TestCategory? Category { get; private set; }

        private TestProduct() { }

        public TestProduct(string name, decimal price, TestCategory? category = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

            Name = name;
            Price = price;
            Category = category;
        }
    }

    public class TestCategory : Entity
    {
        public string Name { get; private set; } = string.Empty;

        private TestCategory() { }

        public TestCategory(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            Name = name;
        }
    }
}
