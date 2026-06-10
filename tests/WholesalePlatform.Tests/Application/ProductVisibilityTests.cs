using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using WholesalePlatform.Application.Common.Exceptions;
using WholesalePlatform.Application.Common.Mappings;
using WholesalePlatform.Application.Products.Queries.GetProducts;
using WholesalePlatform.Domain.Products;
using WholesalePlatform.Domain.Users;
using WholesalePlatform.Tests.TestDoubles;

namespace WholesalePlatform.Tests.Application;

public sealed class ProductVisibilityTests
{
    [Fact]
    public async Task Customer_cannot_list_unavailable_products()
    {
        var customer = User.CreateCustomer(
            "customer@example.com",
            "Customer",
            "hashed-password");
        var product = Product.Create("SKU-1", "Product", null, 10m);
        product.Update("SKU-1", "Product", null, 10m, isAvailable: false);

        var mapper = new MapperConfiguration(
            configuration => configuration.AddProfile<ApplicationMappingProfile>(),
            NullLoggerFactory.Instance).CreateMapper();
        var handler = new GetProductsQueryHandler(
            new FakeUnitOfWork(users: [customer], products: [product]),
            mapper,
            new FakeCurrentUserService { UserId = customer.Id });

        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            handler.Handle(new GetProductsQuery(OnlyAvailable: false), CancellationToken.None));
    }
}
