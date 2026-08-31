using Api.Contracts.Common;
using AutoMapper;
using Domain.Common;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Test.Repositories;

public class RepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly IConfigurationProvider _mapperConfig;
    private readonly Repository<TestProduct> _repository;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestApplicationDbContext(options);

        _mapperConfig = new MapperConfiguration(cfg => {
            cfg.CreateMap<TestProduct, TestProductDto>();
        }, new LoggerFactory());

        _repository = new Repository<TestProduct>(_context, _mapperConfig);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenEntityExists()
    {
        // Arrange
        var product = TestProduct.Create("Laptop", 1000);
        await _context.Set<TestProduct>().AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Laptop", result.Name);
    }

    [Fact]
    public async Task ListAsync_ShouldFilterAndApplyIncludes()
    {
        // Arrange
        var category = TestCategory.Create("Electronics");
        var product1 = TestProduct.Create("Phone", 500, category);
        var product2 = TestProduct.Create("Desk", 200);

        await _context.Set<TestProduct>().AddRangeAsync(product1, product2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ListAsync(
            predicate: p => p.Price > 300,
            includes: p => p.Category!);

        // Assert
        Assert.Single(result);
        Assert.Equal("Phone", result[0].Name);
        Assert.NotNull(result[0].Category);
        Assert.Equal("Electronics", result[0].Category!.Name);
    }

    [Fact]
    public async Task ProjectToPagedAsync_ShouldReturnCorrectPagedResultAndOrdering()
    {
        // Arrange
        var products = Enumerable.Range(1, 10)
            .Select(i => TestProduct.Create($"Product {i}", i * 10))
            .ToList();

        await _context.Set<TestProduct>().AddRangeAsync(products);
        await _context.SaveChangesAsync();

        var pagination = new PaginationRequest { PageNumber = 2, PageSize = 3 };

        // Act
        var result = await _repository.ProjectToPagedAsync<TestProductDto>(
            pagination: pagination,
            orderBy: p => p.Price,
            descending: true);

        // Assert
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal("Product 7", result.Items[0].Name);
        Assert.Equal("Product 5", result.Items[2].Name);
    }

    [Fact]
    public async Task AddAsync_And_Remove_ShouldTrackEntityStateCorrectly()
    {
        // Arrange
        var product = TestProduct.Create("Monitor", 300);

        // Act 1: Add
        await _repository.AddAsync(product);
        await _context.SaveChangesAsync();

        Assert.True(await _repository.ExistsAsync(p => p.Name == "Monitor"));

        // Act 2: Remove
        _repository.Remove(product);
        await _context.SaveChangesAsync();

        Assert.False(await _repository.ExistsAsync(p => p.Name == "Monitor"));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

#region DDD Test Entities & DTOs

public class TestProduct : Entity
{
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public TestCategory? Category { get; private set; }

    // Required by EF Core parameterless constructor binding
    private TestProduct() { }

    private TestProduct(Guid id, string name, decimal price, TestCategory? category = null)
    {
        Id = id;
        Name = name;
        Price = price;
        Category = category;
    }

    // Static Factory Method (DDD invariant entry point)
    public static TestProduct Create(string name, decimal price, TestCategory? category = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

        return new TestProduct(Guid.NewGuid(), name, price, category);
    }
}

public class TestCategory : Entity
{
    public string Name { get; private set; } = string.Empty;

    private TestCategory() { }

    private TestCategory(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public static TestCategory Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new TestCategory(Guid.NewGuid(), name);
    }
}

public class TestProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

#endregion
