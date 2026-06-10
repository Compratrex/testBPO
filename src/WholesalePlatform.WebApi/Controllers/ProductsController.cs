using MediatR;
using Microsoft.AspNetCore.Mvc;
using WholesalePlatform.Application.Common.Models;
using WholesalePlatform.Application.Products.Commands.CreateProduct;
using WholesalePlatform.Application.Products.Commands.DeleteProduct;
using WholesalePlatform.Application.Products.Commands.UpdateProduct;
using WholesalePlatform.Application.Products.Queries.GetProducts;
using WholesalePlatform.Domain.Enums;
using WholesalePlatform.WebApi.Authorization;
using WholesalePlatform.WebApi.Contracts.Common;
using WholesalePlatform.WebApi.Contracts.Products;
using WholesalePlatform.WebApi.Routing;

namespace WholesalePlatform.WebApi.Controllers;

[Route(ApiRoutes.Products.Base)]
public sealed class ProductsController : ApiControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [HasPermission(PermissionCode.ProductsRead)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductDto>>>> GetProducts(
        [FromQuery] bool onlyAvailable = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var products = await _sender.Send(new GetProductsQuery(onlyAvailable, page, pageSize), cancellationToken);
        return OkResponse(products);
    }

    [HttpPost]
    [HasPermission(PermissionCode.ProductsManage)]
    [ProducesResponseType(typeof(ApiResponse<CreatedIdDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<CreatedIdDto>>> CreateProduct(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateProductCommand(request.Sku, request.Name, request.Description, request.Price),
            cancellationToken);

        return CreatedResponse(ApiRoutes.Products.Location(result.Id), result);
    }

    [HttpPut(ApiRoutes.Products.ById)]
    [HasPermission(PermissionCode.ProductsManage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateProduct(
        Guid productId,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new UpdateProductCommand(
                productId,
                request.Sku,
                request.Name,
                request.Description,
                request.Price,
                request.IsAvailable,
                request.Version),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete(ApiRoutes.Products.ById)]
    [HasPermission(PermissionCode.ProductsManage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(Guid productId, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteProductCommand(productId), cancellationToken);
        return NoContent();
    }
}
