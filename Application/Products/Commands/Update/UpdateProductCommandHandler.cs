using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Specifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Commands.Update;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var specification = new ProductByIdSpecification(request.Id);
        var product = await _context.Products
            .Specify(specification)
            .FirstOrDefaultAsync(cancellationToken);

        if (product == null)
            throw new NotFoundException(nameof(Product), request.Id);

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Stock = request.Stock;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
