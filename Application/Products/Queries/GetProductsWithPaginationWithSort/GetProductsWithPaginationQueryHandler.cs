using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Products.Queries.Common;
using AutoMapper;
using Domain.Specifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Queries.GetProductsWithPaginationWithSort;

public class GetProductsWithPaginationQueryHandler
    : IRequestHandler<GetProductsWithPaginationQuery, PaginatedProductsVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProductsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedProductsVm> Handle(GetProductsWithPaginationQuery request, CancellationToken cancellationToken)
    {

        if (request.PageNumber < 1)
            throw new ArgumentException("Page number must be greater than 0");

        if (request.PageSize < 1 || request.PageSize > 100)
            throw new ArgumentException("Page size must be between 1 and 100");

        var specification = new ProductsWithPaginationWithSortSpecification(
            request.PageNumber,
            request.PageSize,
            request.SortBy,
            request.SortDescending);

        var products = await _context.Products
            .Specify(specification)
            .ToListAsync(cancellationToken);

        var totalCount = await _context.Products
            .CountAsync(p => p.IsActive, cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new PaginatedProductsVm
        {
            Products = _mapper.Map<List<ProductDto>>(products),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasPreviousPage = request.PageNumber > 1,
            HasNextPage = request.PageNumber < totalPages
        };
    }
}
