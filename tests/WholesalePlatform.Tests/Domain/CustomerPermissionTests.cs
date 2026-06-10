using WholesalePlatform.Domain.Common;
using WholesalePlatform.Domain.Enums;
using WholesalePlatform.Domain.Permissions;
using WholesalePlatform.Domain.Users;

namespace WholesalePlatform.Tests.Domain;

public sealed class CustomerPermissionTests
{
    [Fact]
    public void CreateCustomer_rejects_management_permissions()
    {
        Assert.Throws<DomainException>(() =>
            User.CreateCustomer(
                "customer@example.com",
                "Customer",
                "hashed-password",
                [PermissionCode.ProductsRead, PermissionCode.CustomersManage]));
    }

    [Fact]
    public void CreateCustomer_uses_safe_default_permissions()
    {
        var customer = User.CreateCustomer(
            "customer@example.com",
            "Customer",
            "hashed-password");

        Assert.All(
            customer.Permissions,
            permission => Assert.True(DefaultPermissions.IsAllowedForCustomer(permission.Permission)));
    }
}
