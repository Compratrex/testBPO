using MediatR;
using WholesalePlatform.Application.Common.Models;

namespace WholesalePlatform.Application.Products.Queries.GetProducts;

public sealed record GetProductsQuery(
    bool OnlyAvailable = true,
    int Page = 1,
    int PageSize = 50) : IRequest<PagedResult<ProductDto>>;
