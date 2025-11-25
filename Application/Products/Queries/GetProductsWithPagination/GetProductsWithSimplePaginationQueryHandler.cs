using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Products.Queries.Common;
using Application.Products.Queries.GetProductsWithPaginationWithSort;
using AutoMapper;
using Domain.Specifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Queries.GetProductsWithPagination;

public class GetProductsWithSimplePaginationQueryHandler: IRequestHandler<GetProductsWithPaginationQuery, PaginatedProductsVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProductsWithSimplePaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedProductsVm> Handle(GetProductsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var specification = new ProductsWithPaginationSpecification(request.PageNumber, request.PageSize);

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
