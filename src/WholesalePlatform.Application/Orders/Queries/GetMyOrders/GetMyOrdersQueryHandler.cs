using AutoMapper;
using MediatR;
using WholesalePlatform.Application.Abstractions.Auth;
using WholesalePlatform.Application.Abstractions.Persistence;
using WholesalePlatform.Application.Common.Models;

namespace WholesalePlatform.Application.Orders.Queries.GetMyOrders;

public sealed class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, PagedResult<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetMyOrdersQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<PagedResult<OrderDto>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var customer = await _unitOfWork.Customers.GetByUserIdAsync(userId, cancellationToken);
        if (customer is null)
        {
            return new PagedResult<OrderDto>([], request.Page, request.PageSize, 0);
        }

        var orders = await _unitOfWork.Orders.ListByCustomerIdAsync(
            customer.Id,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<OrderDto>(
            _mapper.Map<IReadOnlyList<OrderDto>>(orders.Items),
            orders.Page,
            orders.PageSize,
            orders.TotalCount);
    }
}
