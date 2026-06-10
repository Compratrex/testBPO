using WholesalePlatform.Application.Common.Exceptions;
using WholesalePlatform.Application.Orders.Commands.CreateOrder;
using WholesalePlatform.Domain.Products;
using WholesalePlatform.Domain.Users;
using WholesalePlatform.Tests.TestDoubles;

namespace WholesalePlatform.Tests.Application;

public sealed class OrderSecurityTests
{
    [Fact]
    public async Task CreateOrder_rejects_administrator_as_customer()
    {
        var admin = User.CreateAdministrator(
            "admin@example.com",
            "Admin",
            "hashed-password");
        var product = Product.Create("SKU-1", "Product", null, 10m);
        var unitOfWork = new FakeUnitOfWork(users: [admin], products: [product]);
        var handler = new CreateOrderCommandHandler(
            unitOfWork,
            new FakeCurrentUserService { UserId = admin.Id });

        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            handler.Handle(
                new CreateOrderCommand([new CreateOrderItemCommand(product.Id, 1)]),
                CancellationToken.None));
    }
}
