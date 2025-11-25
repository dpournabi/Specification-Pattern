using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Products.Queries.Common;
using AutoMapper;
using Domain.Specifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, ProductsVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ProductsVm> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var specification = new ActiveProductsSpecification();

        var products = await _context.Products
            .Specify(specification)
            .ToListAsync(cancellationToken);

        return new ProductsVm
        {
            Products = _mapper.Map<List<ProductDto>>(products),
            Count = products.Count
        };
    }
}