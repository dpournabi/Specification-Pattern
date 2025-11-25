using Application.Common.Interfaces;
using Application.Products.Commands.Create;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Test.Commands;

public class CreateProductCommandHandlerTests
{
    private readonly IApplicationDbContext _context;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _context = Substitute.For<IApplicationDbContext>();
        _handler = new CreateProductCommandHandler(_context);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateProduct()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "New Product",
            Description = "Product Description",
            Price = 19.99m,
            Stock = 10
        };

        var products = new List<Product>();
        _context.Products.Returns(Substitute.For<DbSet<Product>>());
        _context.Products.When(x => x.Add(Arg.Any<Product>()))
            .Do(callback => products.Add(callback.Arg<Product>()));

        _context.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(1));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        products.Should().HaveCount(1);
        products[0].Name.Should().Be("New Product");
        products[0].Price.Should().Be(19.99m);
        products[0].IsActive.Should().BeTrue();
        await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
