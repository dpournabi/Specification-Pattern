using Application.Products.Queries.GetProductsWithPaginationWithSort;
using MediatR;

namespace Application.Products.Queries.GetProductsWithPagination;

public record GetProductsWithPaginationQuery : IRequest<PaginatedProductsVm>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
