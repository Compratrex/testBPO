using WholesalePlatform.Application.Customers.Commands.DeleteCustomer;
using WholesalePlatform.Domain.Common;
using WholesalePlatform.Domain.Customers;
using WholesalePlatform.Domain.Orders;
using WholesalePlatform.Domain.Users;
using WholesalePlatform.Tests.TestDoubles;

namespace WholesalePlatform.Tests.Application;

public sealed class CustomerDeletionTests
{
    [Fact]
    public async Task DeleteCustomer_rejects_customer_with_active_orders()
    {
        var user = User.CreateCustomer(
            "customer@example.com",
            "Customer",
            "hashed-password");
        var customer = Customer.Create("Customer", "Legal address", user.Id);
        var order = Order.Create(customer.Id, user.Id);
        order.AddItem(Guid.NewGuid(), "SKU-1", "Product", 10m, 1);
        var handler = new DeleteCustomerCommandHandler(
            new FakeUnitOfWork(users: [user], customers: [customer], orders: [order]));

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.Handle(new DeleteCustomerCommand(customer.Id), CancellationToken.None));
    }

    [Fact]
    public async Task DeleteCustomer_removes_customer_login_when_no_active_orders()
    {
        var user = User.CreateCustomer(
            "customer@example.com",
            "Customer",
            "hashed-password");
        var customer = Customer.Create("Customer", "Legal address", user.Id);
        var unitOfWork = new FakeUnitOfWork(users: [user], customers: [customer]);
        var handler = new DeleteCustomerCommandHandler(unitOfWork);

        await handler.Handle(new DeleteCustomerCommand(customer.Id), CancellationToken.None);

        Assert.Null(await unitOfWork.Customers.GetAccountByIdAsync(customer.Id, CancellationToken.None));
        Assert.Null(await unitOfWork.Users.GetByIdAsync(user.Id, CancellationToken.None));
        Assert.Equal(1, unitOfWork.SaveChangesCount);
    }
}
