using Application.Products.Queries.Common;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.Mappings;

public class ProductMapping : Profile
{
    public ProductMapping()
    {
        CreateMap<Product, ProductDto>();
    }
}
