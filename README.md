# Specification Pattern in C#

### (With Explanation of Why It Can Replace Repository + Unit of Work Pattern)

------------------------------------------------------------------------

## 📌 Overview

This project implements a modern, clean, expression‑based
**Specification Pattern** in C#.\
The goal is to **centralize business rules**, **improve query
composability**, and reduce the complexity often caused by overusing
**Repository + Unit of Work** patterns.

------------------------------------------------------------------------

# 🧠 Why Specification Pattern?

Traditional architectures often use:

-   **Repository Pattern** → hides data access\
-   **Unit of Work** → manages transactions

While useful in some scenarios, these patterns develop serious problems
in large applications:

### ❌ Problems With Standard Repository Pattern

1.  **Method explosion**\
    `GetActiveCustomers`, `GetByEmail`, `GetByRegionAndBalance`,
    `GetByRoleAndRegistrationDate`, etc.\
    This quickly becomes unmanageable.

2.  **Repository is polluted with business rules**\
    Every new query means adding a new method → violates **Open/Closed
    Principle**.

3.  **No composability**\
    You can't combine repository methods like:\
    `GetCustomersByAge()` AND `GetCustomersWithDebt()`.

4.  **Repositories become anemic wrappers around EF Core** Most
    repositories simply return `_context.Set<T>()`.\
    This adds no actual abstraction.

### ❌ Problems With Unit of Work Pattern

1.  EF Core **already *is* a Unit of Work**\
    `DbContext.SaveChanges()` *is literally* a unit of work.\
2.  Maintaining a separate UoW layer = **unnecessary boilerplate**
3.  Adds no real value unless using multiple databases or DDD modules

------------------------------------------------------------------------

# ✅ Why Specification Pattern Is Better

Instead of building more repository methods, you encapsulate business
rules into **small reusable objects** called Specifications.

### ✔ Encapsulated Business Rules

``` csharp
var specification = new ActiveProductsSpecification();
```

This object **represents** the rule.

------------------------------------------------------------------------

### ✔ Composable Rules (AND / OR / NOT)

``` csharp
 public async Task<ProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
 {
     var specification = new ProductByIdSpecification(request.Id)
         .And(new ActiveProductsSpecification());

     var product = await _context.Products
         .Specify(specification)
         .FirstOrDefaultAsync(cancellationToken);

     if (product == null)
         throw new NotFoundException(nameof(Product), request.Id);

     return _mapper.Map<ProductDto>(product);
 }
```

------------------------------------------------------------------------

### ✔ Strongly Typed Expressions (Can Be Translated to SQL)

Specifications use:

``` csharp
Expression<Func<T, bool>> Criteria
```

EF Core can translate them into SQL → highly efficient.

------------------------------------------------------------------------

### ✔ Keeps Domain Logic Out of Repository

Business rules stay in Domain layer.\

------------------------------------------------------------------------

### ✔ Highly Testable

Unit tests can verify business rules without database.

------------------------------------------------------------------------

# 🏗 Project Architecture

    Domain
    │── Entities
    │── BaseSpecification.cs
    │── ISpecification.cs
    │── Concrete Specifications
    Application
    │── Services & Handlers
    Infrastructure
    │── ApplicationDbContext
    WebApi
    │── Controllers
    Tests
    │── Specification Tests
    │── Spec Integration Tests

------------------------------------------------------------------------

# 🧱 Core Specification Interface

``` csharp
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
```

------------------------------------------------------------------------

# 🧩 Base Specification Implementation

``` csharp
public abstract class BaseSpecification<T> : ISpecification<T>
{
    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    protected BaseSpecification() { }

    public Expression<Func<T, bool>> Criteria { get; } = null!;
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public Expression<Func<T, object>> OrderBy { get; private set; } = null!;
    public Expression<Func<T, object>> OrderByDescending { get; private set; } = null!;
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }

    protected virtual void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }
    public ISpecification<T> And(ISpecification<T> other)
    {
        var combined = ExpressionComposer.And(this.Criteria, other.Criteria);
        return new DirectSpecification<T>(combined);
    }

    public ISpecification<T> Or(ISpecification<T> other)
    {
        var combined = ExpressionComposer.Or(this.Criteria, other.Criteria);
        return new DirectSpecification<T>(combined);
    }

    public ISpecification<T> Not()
    {
        var combined = ExpressionComposer.Not(this.Criteria);
        return new DirectSpecification<T>(combined);
    }
}

```

------------------------------------------------------------------------

# ⚙ Specification Evaluator

This class applies specification rules to EF queries:

``` csharp
public static class SpecificationExtensions
{
    public static IQueryable<T> Specify<T>(this IQueryable<T> query, ISpecification<T> specification)
        where T : class
    {
        // Filter criteria
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // Includes
        query = specification.Includes.Aggregate(query,
            (current, include) => current.Include(include));

        // Ordering
        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        // Paging
        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip).Take(specification.Take);
        }

        return query;
    }
}
```

------------------------------------------------------------------------

# 🧪 Testing Specifications

Easy to test:

``` csharp
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
```

------------------------------------------------------------------------

# 🆚 Specification Pattern vs. Repository + Unit of Work

<p> Feature                            Repository+UoW   Specification Pattern
   ---------------------------------- ---------------- ----------------------- <br>
   Avoids method explosion            ❌ No            ✔ Yes <br>
   Encapsulates business rules        ❌ No            ✔ Yes <br>
   Composable (AND/OR/NOT)            ❌ No            ✔ Yes <br>
   EF Core integration                ✔ Good           ✔ Excellent <br>
   Testability                        Medium           High <br>
   Clean architecture alignment       Medium           High <br>
   Boilerplate code                   High             Low <br>
   Supports complex dynamic queries   Poor             Excellent <br>

------------------------------------------------------------------------ </p>

# 🏁 Running the Project

``` bash
git clone https://github.com/dpournabi/Specification-Pattern
cd Specification-Pattern
dotnet restore
dotnet build
dotnet test
```
