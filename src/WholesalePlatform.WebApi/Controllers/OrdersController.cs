using MediatR;
using Microsoft.AspNetCore.Mvc;
using WholesalePlatform.Application.Common.Models;
using WholesalePlatform.Application.Orders.Commands.CancelOrder;
using WholesalePlatform.Application.Orders.Commands.CreateOrder;
using WholesalePlatform.Application.Orders.Queries.GetMyOrders;
using WholesalePlatform.Domain.Enums;
using WholesalePlatform.WebApi.Authorization;
using WholesalePlatform.WebApi.Contracts.Common;
using WholesalePlatform.WebApi.Contracts.Orders;
using WholesalePlatform.WebApi.Routing;

namespace WholesalePlatform.WebApi.Controllers;

[Route(ApiRoutes.Orders.Base)]
public sealed class OrdersController : ApiControllerBase
{
    private readonly ISender _sender;

    public OrdersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet(ApiRoutes.Orders.My)]
    [HasPermission(PermissionCode.OrdersReadOwn)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OrderDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<PagedResult<OrderDto>>>> GetMyOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var orders = await _sender.Send(new GetMyOrdersQuery(page, pageSize), cancellationToken);
        return OkResponse(orders);
    }

    [HttpPost]
    [HasPermission(PermissionCode.OrdersCreate)]
    [ProducesResponseType(typeof(ApiResponse<CreatedIdDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CreatedIdDto>>> CreateOrder(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateOrderCommand(
                request.Items
                    .Select(item => new CreateOrderItemCommand(item.ProductId, item.Quantity))
                    .ToArray()),
            cancellationToken);

        return CreatedResponse(ApiRoutes.Orders.Location(result.Id), result);
    }

    [HttpPost(ApiRoutes.Orders.Cancel)]
    [HasPermission(PermissionCode.OrdersCancelOwn)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CancelOrder(Guid orderId, CancellationToken cancellationToken)
    {
        await _sender.Send(new CancelOrderCommand(orderId), cancellationToken);
        return NoContent();
    }
}
