Clean Architecture with Specification Pattern
🚀 Project Overview
This project demonstrates a modern .NET 9 Clean Architecture implementation that uses the Specification Pattern instead of traditional Repository and Unit of Work patterns. The approach provides a more flexible, maintainable, and powerful way to handle data access while maintaining clean separation of concerns.

📋 Table of Contents
Why Specification Pattern?

Benefits Over Repository Pattern

Architecture Overview

Key Components

Usage Examples

Comparison with Repository Pattern

Getting Started

🎯 Why Specification Pattern?
The Specification Pattern encapsulates business rules that determine whether an object does or does not satisfy some criteria. When applied to data access, it provides a more flexible and composable alternative to the Repository Pattern.

Traditional Repository Pattern Issues:
Rigid interfaces with methods like GetById, GetAll, Find

Repository explosion - multiple repositories for each entity

Complex queries end up in service layers

Hard to compose and reuse query logic

Leaky abstractions - repository becomes a thin wrapper over DbContext

✨ Benefits Over Repository Pattern
1. True Business Logic Encapsulation
csharp
// Specifications encapsulate business rules
public class ActiveProductsByPriceRangeSpecification : BaseSpecification<Product>
{
    public ActiveProductsByPriceRangeSpecification(decimal minPrice, decimal maxPrice)
        : base(p => p.IsActive && p.Price >= minPrice && p.Price <= maxPrice)
    {
        ApplyOrderBy(p => p.Price);
        AddInclude(p => p.Category);
    }
}

// Usage in query handler
var specification = new ActiveProductsByPriceRangeSpecification(10, 100);
var products = await _context.Products.Specify(specification).ToListAsync();
2. Composable and Reusable
csharp
// Combine specifications
var cheapProducts = new ProductsByPriceRangeSpecification(0, 50);
var inStockProducts = new ProductsInStockSpecification();
var combinedSpec = cheapProducts.And(inStockProducts);

// Reuse across different queries
var paginatedSpec = new ProductsWithPaginationSpecification(1, 10);
var expensivePaginatedSpec = new ProductsByPriceRangeSpecification(100, 1000)
    .WithPagination(1, 10);
3. No Repository Explosion
csharp
// Instead of multiple repositories:
// - IProductRepository
// - IOrderRepository  
// - IUserRepository

// We use DbContext directly with specifications:
var products = await _context.Products.Specify(spec).ToListAsync();
var orders = await _context.Orders.Specify(spec).ToListAsync();
var users = await _context.Users.Specify(spec).ToListAsync();
4. Better Testability
csharp
// Easy to test specifications in isolation
[Fact]
public void ActiveProductsSpecification_Should_Return_Only_Active_Products()
{
    // Arrange
    var spec = new ActiveProductsSpecification();
    var products = new List<Product>
    {
        new() { IsActive = true },
        new() { IsActive = false }
    }.AsQueryable();

    // Act
    var result = products.Specify(spec).ToList();

    // Assert
    result.Should().HaveCount(1);
    result.Should().OnlyContain(p => p.IsActive);
}
5. EF Core Integration
csharp
// Leverage full EF Core power without abstraction
var products = await _context.Products
    .Specify(specification)
    .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
    .ToListAsync();

// Complex queries with joins, includes, and projections
var spec = new ProductWithCategoryAndReviewsSpecification();
var result = await _context.Products
    .Specify(spec)
    .Select(p => new ProductSummaryDto
    {
        Name = p.Name,
        Category = p.Category.Name,
        AverageRating = p.Reviews.Average(r => r.Rating)
    })
    .ToListAsync();
🏗 Architecture Overview
text
Specification-Pattern/
├── Domain/
│   ├── Entities/           # Domain entities
│   └── Specifications/     # Business specifications
├── Application/
│   ├── Common/
│   │   └── Extensions/     # Specification extensions
│   ├── Products/
│   │   ├── Commands/       # CQRS Commands
│   │   └── Queries/        # CQRS Queries
│   └── DependencyInjection.cs
├── Infrastructure/
│   └── Data/              # DbContext and configurations
└── Web/
    └── Controllers/       # API Controllers
🔑 Key Components
1. Specification Interface
csharp
public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    Expression<Func<T, object>> OrderBy { get; }
    Expression<Func<T, object>> OrderByDescending { get; }
    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; }
}
2. Base Specification
csharp
public abstract class BaseSpecification<T> : ISpecification<T>
{
    protected BaseSpecification(Expression<Func<T, bool>> criteria) => Criteria = criteria;
    protected BaseSpecification() { }

    // Implementation with fluent methods for building specifications
    protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    protected virtual void ApplyPaging(int skip, int take)
}
3. Specification Extensions
csharp
public static class SpecificationExtensions
{
    public static IQueryable<T> Specify<T>(this IQueryable<T> query, ISpecification<T> specification)
    {
        // Apply criteria, includes, ordering, and paging
        return query;
    }
}
📖 Usage Examples
Simple Query
csharp
public class GetActiveProductsQueryHandler : IRequestHandler<GetActiveProductsQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(GetActiveProductsQuery request, CancellationToken cancellationToken)
    {
        var spec = new ActiveProductsSpecification();
        var products = await _context.Products
            .Specify(spec)
            .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
            
        return products;
    }
}
Complex Business Query
csharp
public class GetProductsForDashboardQueryHandler 
    : IRequestHandler<GetProductsForDashboardQuery, DashboardVm>
{
    public async Task<DashboardVm> Handle(GetProductsForDashboardQuery request, CancellationToken cancellationToken)
    {
        var lowStockSpec = new LowStockProductsSpecification(threshold: 10);
        var expensiveProductsSpec = new ProductsByPriceRangeSpecification(100, decimal.MaxValue);
        var recentProductsSpec = new RecentProductsSpecification(TimeSpan.FromDays(30));
        
        var lowStockCount = await _context.Products.Specify(lowStockSpec).CountAsync(cancellationToken);
        var expensiveProducts = await _context.Products.Specify(expensiveProductsSpec).ToListAsync(cancellationToken);
        
        return new DashboardVm { /* ... */ };
    }
}
⚖️ Comparison with Repository Pattern
Aspect	Repository Pattern	Specification Pattern
Flexibility	Limited by repository interface	Highly flexible and composable
Business Logic	Often leaks to service layer	Encapsulated in specifications
Testability	Requires mocking repositories	Easy to test specifications in isolation
Complex Queries	End up in service layer or custom repo methods	Naturally handled by specifications
EF Core Features	Often hidden behind repository abstraction	Full access to EF Core capabilities
Code Reuse	Limited to repository methods	High reuse through composition
Maintenance	Repository explosion problem	Centralized and maintainable
🚀 Getting Started
1. Clone and Setup
bash
[git clone <repository-url>](https://github.com/dpournabi/Specification-Pattern.git)
cd Specification-Pattern
dotnet restore
2. Database Setup
bash
dotnet ef database update
3. Run the Application
bash
dotnet run --project src/WebApi
4. API Endpoints
GET /api/products - Get all products

GET /api/products/paginated - Get paginated products

GET /api/products/by-price - Get products by price range

POST /api/products - Create new product

PUT /api/products/{id} - Update product

DELETE /api/products/{id} - Delete product

🧪 Testing Specifications
csharp
public class ProductSpecificationsTests
{
    [Fact]
    public void ProductByIdSpecification_Should_Filter_By_Id()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var spec = new ProductByIdSpecification(productId);
        var products = new List<Product>
        {
            new() { Id = productId, Name = "Test Product" },
            new() { Id = Guid.NewGuid(), Name = "Other Product" }
        }.AsQueryable();

        // Act
        var result = products.Specify(spec).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(productId);
    }
}
📚 Best Practices
Keep specifications focused on single responsibility

Compose complex queries from simple specifications

Use meaningful names that describe business intent

Test specifications in isolation

Leverage EF Core features directly when needed

Combine with CQRS for clean separation

🎉 Conclusion
The Specification Pattern provides a superior alternative to the Repository Pattern by:

✅ Encapsulating business rules effectively

✅ Enabling composition and reuse

✅ Leveraging EF Core without unnecessary abstraction

✅ Improving testability and maintainability

✅ Preventing repository explosion

This approach aligns perfectly with Clean Architecture principles and provides a more natural way to express complex business queries while maintaining separation of concerns.

