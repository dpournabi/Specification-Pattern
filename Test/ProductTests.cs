using Domain.Entities;
using Test.Fixtures;

namespace Test;

public class ProductTests : IClassFixture<DbContextFixture>
{
    private readonly DbContextFixture _fixture;

    public ProductTests(DbContextFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void SampleIntegrationTest()
    {
        // Arrange
        var product = new Product { Name = "Test", IsActive = true };

        // Act
        _fixture.Context.Products.Add(product);
        _fixture.Context.SaveChanges();

        // Assert
        Assert.Single(_fixture.Context.Products);
    }
}
