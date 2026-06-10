using AutoMapper;
using WholesalePlatform.Application.Common.Models;
using WholesalePlatform.Domain.Orders;
using WholesalePlatform.Domain.Products;

namespace WholesalePlatform.Application.Common.Mappings;

public sealed class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<Product, ProductDto>();

        CreateMap<Order, OrderDto>();
        CreateMap<OrderItem, OrderItemDto>();
    }
}
