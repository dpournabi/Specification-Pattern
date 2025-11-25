using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Test.Fixtures;

public class DbContextFixture : IDisposable
{
    public ApplicationDbContext Context { get; }

    public DbContextFixture()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        Context = new ApplicationDbContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}

public class ProductTestData
{
    public static List<Product> GetTestProducts()
    {
        return new List<Product>
        {
            new() {
                Id = Guid.NewGuid(),
                Name = "Laptop",
                Price = 999.99m,
                Stock = 5,
                IsActive = true
            },
            new() {
                Id = Guid.NewGuid(),
                Name = "Mouse",
                Price = 25.50m,
                Stock = 20,
                IsActive = true
            },
            new() {
                Id = Guid.NewGuid(),
                Name = "Keyboard",
                Price = 75.00m,
                Stock = 0,
                IsActive = false
            }
        };
    }
}