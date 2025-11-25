using Application.Products.Commands.Create;
using Application.Products.Commands.Delete;
using Application.Products.Commands.Update;
using Application.Products.Queries.Common;
using Application.Products.Queries.GetProduct;
using Application.Products.Queries.GetProducts;
using Application.Products.Queries.GetProductsByPrice;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<ProductsVm>> GetProducts()
    {
        var result = await _sender.Send(new GetProductsQuery());
        return Ok(result);
    }

    [HttpGet("by-price")]
    public async Task<ActionResult<ProductsVm>> GetProductsByPrice([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {
        var result = await _sender.Send(new GetProductsByPriceQuery(minPrice, maxPrice));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        var result = await _sender.Send(new GetProductQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateProduct(CreateProductCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateProduct(Guid id, UpdateProductCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        await _sender.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteProduct(Guid id)
    {
        await _sender.Send(new DeleteProductCommand(id));
        return NoContent();
    }
}
