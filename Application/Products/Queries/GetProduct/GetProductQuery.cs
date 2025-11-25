using Application.Products.Queries.Common;
using MediatR;

namespace Application.Products.Queries.GetProduct;
public record GetProductQuery(Guid Id) : IRequest<ProductDto>;
