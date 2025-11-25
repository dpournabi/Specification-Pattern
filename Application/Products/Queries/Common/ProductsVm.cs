namespace Application.Products.Queries.Common;

public class ProductsVm
{
    public IList<ProductDto> Products { get; set; } = new List<ProductDto>();
    public int Count { get; set; }
}
