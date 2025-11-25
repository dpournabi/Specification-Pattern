using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Products.Queries.Common;
using AutoMapper;
using Domain.Entities;
using Domain.Specifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Queries.GetProduct;

public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ProductDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProductQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var specification = new ProductByIdSpecification(request.Id);

        var product = await _context.Products
            .Specify(specification)
            .FirstOrDefaultAsync(cancellationToken);

        if (product == null)
            throw new NotFoundException(nameof(Product), request.Id);

        return _mapper.Map<ProductDto>(product);
    }
}
