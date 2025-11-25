using Application.Products.Queries.Common;

namespace Application.Products.Queries.GetProductsWithPaginationWithSort;

public class PaginatedProductsVm
{
    public IList<ProductDto> Products { get; set; } = new List<ProductDto>();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}
