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

public class ProductsWithPaginationWithSortSpecification : BaseSpecification<Product>
{
    public ProductsWithPaginationWithSortSpecification(int pageNumber, int pageSize, string? sortBy = null, bool sortDescending = false)
        : base(p => p.IsActive)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);

        if (!string.IsNullOrEmpty(sortBy))
        {
            switch (sortBy.ToLower())
            {
                case "name":
                    if (sortDescending)
                        ApplyOrderByDescending(p => p.Name);
                    else
                        ApplyOrderBy(p => p.Name);
                    break;
                case "price":
                    if (sortDescending)
                        ApplyOrderByDescending(p => p.Price);
                    else
                        ApplyOrderBy(p => p.Price);
                    break;
                case "stock":
                    if (sortDescending)
                        ApplyOrderByDescending(p => p.Stock);
                    else
                        ApplyOrderBy(p => p.Stock);
                    break;
                default:
                    if (sortDescending)
                        ApplyOrderByDescending(p => p.CreatedAt);
                    else
                        ApplyOrderBy(p => p.CreatedAt);
                    break;
            }
        }
        else
        {
            ApplyOrderByDescending(p => p.CreatedAt);
        }
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