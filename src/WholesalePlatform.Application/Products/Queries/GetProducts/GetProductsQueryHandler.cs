using AutoMapper;
using MediatR;
using WholesalePlatform.Application.Abstractions.Auth;
using WholesalePlatform.Application.Abstractions.Persistence;
using WholesalePlatform.Application.Common.Exceptions;
using WholesalePlatform.Application.Common.Models;
using WholesalePlatform.Domain.Enums;

namespace WholesalePlatform.Application.Products.Queries.GetProducts;

public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetProductsQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        if (!request.OnlyAvailable)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var user = await _unitOfWork.Users.GetByIdAsync(
                userId,
                cancellationToken,
                includePermissions: true);

            if (user is null || !user.HasPermission(PermissionCode.ProductsManage))
            {
                throw new ForbiddenAccessException("Only product managers can list unavailable products.");
            }
        }

        var products = await _unitOfWork.Products.ListAsync(
            request.OnlyAvailable,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<ProductDto>(
            _mapper.Map<IReadOnlyList<ProductDto>>(products.Items),
            products.Page,
            products.PageSize,
            products.TotalCount);
    }
}
