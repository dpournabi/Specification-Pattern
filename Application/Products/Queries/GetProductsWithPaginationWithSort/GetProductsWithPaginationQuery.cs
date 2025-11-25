using MediatR;

namespace Application.Products.Queries.GetProductsWithPaginationWithSort;

public record GetProductsWithPaginationQuery : IRequest<PaginatedProductsVm>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; } = false;
}
