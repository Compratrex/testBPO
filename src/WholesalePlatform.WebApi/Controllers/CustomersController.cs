using MediatR;
using Microsoft.AspNetCore.Mvc;
using WholesalePlatform.Application.Common.Models;
using WholesalePlatform.Application.Customers.Commands.DeleteCustomer;
using WholesalePlatform.Application.Customers.Commands.RegisterCustomer;
using WholesalePlatform.Application.Customers.Commands.UpdateCustomer;
using WholesalePlatform.Application.Customers.Queries.GetCustomers;
using WholesalePlatform.Domain.Enums;
using WholesalePlatform.WebApi.Authorization;
using WholesalePlatform.WebApi.Contracts.Common;
using WholesalePlatform.WebApi.Contracts.Customers;
using WholesalePlatform.WebApi.Routing;

namespace WholesalePlatform.WebApi.Controllers;

[Route(ApiRoutes.Customers.Base)]
public sealed class CustomersController : ApiControllerBase
{
    private readonly ISender _sender;

    public CustomersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [HasPermission(PermissionCode.CustomersRead)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CustomerDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<PagedResult<CustomerDto>>>> GetCustomers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var customers = await _sender.Send(new GetCustomersQuery(page, pageSize), cancellationToken);
        return OkResponse(customers);
    }

    [HttpPost]
    [HasPermission(PermissionCode.CustomersManage)]
    [ProducesResponseType(typeof(ApiResponse<CreatedIdDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<CreatedIdDto>>> RegisterCustomer(
        [FromBody] RegisterCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new RegisterCustomerCommand(
                request.Email,
                request.FullName,
                request.LegalAddress,
                request.Permissions),
            cancellationToken);

        return CreatedResponse(ApiRoutes.Customers.Location(result.Id), result);
    }

    [HttpPut(ApiRoutes.Customers.ById)]
    [HasPermission(PermissionCode.CustomersManage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateCustomer(
        Guid customerId,
        [FromBody] UpdateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new UpdateCustomerCommand(
                customerId,
                request.FullName,
                request.LegalAddress,
                request.Permissions,
                request.Version),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete(ApiRoutes.Customers.ById)]
    [HasPermission(PermissionCode.CustomersManage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteCustomer(Guid customerId, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteCustomerCommand(customerId), cancellationToken);
        return NoContent();
    }
}
