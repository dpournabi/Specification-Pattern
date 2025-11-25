using Application.Common.Extensions;
using Domain.Entities;
using Domain.Specifications;
using FluentAssertions;

namespace Test.Specifications;

public class ProductSpecificationTests
{
    private readonly List<Product> _products;

    public ProductSpecificationTests()
    {
        _products = new List<Product>
        {
            new() {
                Id = Guid.NewGuid(),
                Name = "Product 1",
                Price = 10.99m,
                Stock = 5,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new() {
                Id = Guid.NewGuid(),
                Name = "Product 2",
                Price = 25.50m,
                Stock = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new() {
                Id = Guid.NewGuid(),
                Name = "Product 3",
                Price = 5.99m,
                Stock = 15,
                IsActive = false,
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new() {
                Id = Guid.NewGuid(),
                Name = "Product 4",
                Price = 100.00m,
                Stock = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };
    }

    [Fact]
    public void ProductByIdSpecification_Should_Filter_By_Id_And_Active_Status()
    {
        // Arrange
        var productId = _products[0].Id;
        var spec = new ProductByIdSpecification(productId);
        var query = _products.AsQueryable();

        // Act
        var result = query.Specify(spec).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(productId);
        result[0].IsActive.Should().BeTrue();
    }

    [Fact]
    public void ActiveProductsSpecification_Should_Return_Only_Active_Products()
    {
        // Arrange
        var spec = new ActiveProductsSpecification();
        var query = _products.AsQueryable();

        // Act
        var result = query.Specify(spec).ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().OnlyContain(p => p.IsActive);
    }

    [Fact]
    public void ProductsByPriceRangeSpecification_Should_Filter_By_Price_Range_And_Order_By_Price()
    {
        // Arrange
        var spec = new ProductsByPriceRangeSpecification(10, 50);
        var query = _products.AsQueryable();

        // Act
        var result = query.Specify(spec).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.Price >= 10 && p.Price <= 50);
        result.Should().BeInAscendingOrder(p => p.Price);
    }
}