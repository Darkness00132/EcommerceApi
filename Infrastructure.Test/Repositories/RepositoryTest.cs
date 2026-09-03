using Api.Contracts.Common;
using AutoMapper;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static TestApplicationDbContext;

namespace Infrastructure.Test.Repositories;

public class RepositoryTests : IDisposable
{
    private readonly TestApplicationDbContext _context;
    private readonly IConfigurationProvider _mapperConfig;
    private readonly Repository<TestProduct> _sut;

    public RepositoryTests()
    {
        var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TestApplicationDbContext(dbOptions);

        _mapperConfig = new MapperConfiguration(cfg => {
            cfg.CreateMap<TestProduct, TestProductDto>();
        }, new LoggerFactory());

        _sut = new Repository<TestProduct>(_context, _mapperConfig);
    }

    [Fact]
    public async Task An_Entity_Can_Be_Retrieved_By_Its_Id()
    {
        // Arrange
        var entity = CreateTestProduct();
        await AddProductOrManyProducts(entity);

        // Act
        var result = await _sut.GetByIdAsync(entity.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
    }

    [Fact]
    public async Task Entities_Can_Be_Retrieved_Using_A_Condition()
    {
        // Arrange
        var entities = CreateTestEntities();
        await AddProductOrManyProducts(entities);

        // Act
        var result = await _sut.ListAsync(x => x.Name == "Test Entity");

        // Assert
        result.Should().ContainSingle();
        result.First().Id.Should().Be(entities.First().Id);
    }

    [Fact]
    public async Task An_Entity_Can_Be_Retrieved_With_Its_Related_Data()
    {
        // Arrange
        var category = CreateTestCategory();
        var entity = CreateTestProduct(category);

        await AddCategories(category);
        await AddProductOrManyProducts(entity);

        // Clear tracked entities so this test verifies that Include loads the related data.
        _context.ChangeTracker.Clear();

        // Act
        var result = await _sut.SingleOrDefaultAsync(
            x => x.Id == entity.Id,
            includes: x => x.Category!);

        // Assert
        result.Should().NotBeNull();
        result.Category.Should().NotBeNull();
        result.Category.Id.Should().Be(category.Id);
    }

    [Fact]
    public async Task An_Entity_Can_Be_Projected_To_Another_Type()
    {
        // Arrange
        var entity = CreateTestProduct();
        await AddProductOrManyProducts(entity);

        // Act
        var result = await _sut.ProjectToSingleOrDefaultAsync<TestProductDto>(
            x => x.Id == entity.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
        result.Name.Should().Be(entity.Name);
        result.Price.Should().Be(entity.Price);
    }

    [Fact]
    public async Task A_Page_Of_Entities_Can_Be_Retrieved()
    {
        // Arrange
        var entities = CreateTestEntities();
        await AddProductOrManyProducts(entities);

        var pagination = new PaginationRequest {
            PageNumber = 1,
            PageSize = 2
        };

        // Act
        var result = await _sut.ProjectToPagedAsync<TestProductDto>(
            pagination,
            x => x.Id);

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task It_Can_Be_Checked_Whether_An_Entity_Exists()
    {
        // Arrange
        var entity = CreateTestProduct();
        await AddProductOrManyProducts(entity);

        // Act
        var result = await _sut.ExistsAsync(x => x.Id == entity.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task An_Entity_Can_Be_Removed()
    {
        // Arrange
        var entity = CreateTestProduct();
        await AddProductOrManyProducts(entity);

        // Act
        _sut.Remove(entity);
        await _context.SaveChangesAsync();

        // Assert
        var storedEntity = await _context.TestProducts.FindAsync(entity.Id);

        storedEntity.Should().BeNull();
    }

    private static TestProduct CreateTestProduct(TestCategory? category = null)
    {
        return new TestProduct("Test Entity", 100m, category);
    }

    private static TestProduct[] CreateTestEntities()
    {
        return
        [
            new TestProduct("Test Entity", 100m),
            new TestProduct("Another Entity", 200m),
            new TestProduct("Third Entity", 300m)
        ];
    }

    private static TestCategory CreateTestCategory()
    {
        return new TestCategory("Test Category");
    }

    private async Task AddProductOrManyProducts(params TestProduct[] entities)
    {
        await _context.TestProducts.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    private async Task AddCategories(params TestCategory[] categories)
    {
        await _context.TestCategories.AddRangeAsync(categories);
        await _context.SaveChangesAsync();
    }

    public void Dispose() => _context.Dispose();

    public class TestProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
