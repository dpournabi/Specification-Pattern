using Application.Products.Queries.Common;
using MediatR;

namespace Application.Products.Queries.GetProducts;

public record GetProductsQuery : IRequest<ProductsVm>;
