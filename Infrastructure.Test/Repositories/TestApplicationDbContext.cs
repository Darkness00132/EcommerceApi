using Infrastructure.Persistence;
using Infrastructure.Test.Repositories;
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
}
