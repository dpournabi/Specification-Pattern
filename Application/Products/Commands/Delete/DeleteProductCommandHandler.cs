using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Specifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Commands.Delete;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var specification = new ProductByIdSpecification(request.Id);
        var product = await _context.Products
            .Specify(specification)
            .FirstOrDefaultAsync(cancellationToken);

        if (product == null)
            throw new NotFoundException(nameof(Product), request.Id);

        product.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);
    }
}