using MediatR;

namespace Application.Products.Commands.Delete;

public record DeleteProductCommand(Guid Id) : IRequest;