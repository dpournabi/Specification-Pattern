using Domain.Entities;
using FluentAssertions;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Products.Queries.GetProduct;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Application.Products.Queries.Common;
using Castle.Core.Logging;

namespace Test.Queries;

public class GetProductQueryHandlerTests
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly GetProductQueryHandler _handler;

    public GetProductQueryHandlerTests()
    {
        _context = CreateDbContext();
        _mapper = CreateMapper();
        _handler = new GetProductQueryHandler(_context, _mapper);
    }

    [Fact]
    public async Task Handle_WithValidId_ShouldReturnProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Price = 29.99m,
            IsActive = true
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(default);

        var query = new GetProductQuery(productId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(productId);
        result.Name.Should().Be("Test Product");
    }

    [Fact]
    public async Task Handle_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        var query = new GetProductQuery(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithInactiveProduct_ShouldThrowNotFoundException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Inactive Product",
            IsActive = false
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(default);

        var query = new GetProductQuery(productId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }

    private static IApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static IMapper CreateMapper()
    {
        var mockLoggerFactory = NSubstitute.Substitute.For<Microsoft.Extensions.Logging.ILoggerFactory>();
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Product, ProductDto>();
        }, mockLoggerFactory);

        return configuration.CreateMapper();
    }
}