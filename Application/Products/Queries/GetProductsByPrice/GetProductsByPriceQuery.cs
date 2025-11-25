using Application.Products.Queries.Common;
using MediatR;

namespace Application.Products.Queries.GetProductsByPrice;

public record GetProductsByPriceQuery(decimal MinPrice, decimal MaxPrice) : IRequest<ProductsVm>;
