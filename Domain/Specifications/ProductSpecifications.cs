using Domain.Entities;

namespace Domain.Specifications;

public class ProductByIdSpecification : BaseSpecification<Product>
{
    public ProductByIdSpecification(Guid id)
        : base(p => p.Id == id && p.IsActive)
    {
    }
}

public class ActiveProductsSpecification : BaseSpecification<Product>
{
    public ActiveProductsSpecification()
        : base(p => p.IsActive)
    {
    }
}

public class ProductsByPriceRangeSpecification : BaseSpecification<Product>
{
    public ProductsByPriceRangeSpecification(decimal minPrice, decimal maxPrice)
        : base(p => p.Price >= minPrice && p.Price <= maxPrice && p.IsActive)
    {
        ApplyOrderBy(p => p.Price);
    }
}

public class ProductsWithPaginationSpecification : BaseSpecification<Product>
{
    public ProductsWithPaginationSpecification(int pageNumber, int pageSize)
        : base(p => p.IsActive)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        ApplyOrderByDescending(p => p.CreatedAt);
    }
}

public class ProductsByNameSpecification : BaseSpecification<Product>
{
    public ProductsByNameSpecification(string name)
        : base(p => p.Name.Contains(name) && p.IsActive)
    {
        ApplyOrderBy(p => p.Name);
    }
}

public class LowStockProductsSpecification : BaseSpecification<Product>
{
    public LowStockProductsSpecification(int threshold = 10)
        : base(p => p.Stock <= threshold && p.IsActive)
    {
        ApplyOrderBy(p => p.Stock);
    }
}