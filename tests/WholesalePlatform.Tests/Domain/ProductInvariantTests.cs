using WholesalePlatform.Domain.Common;
using WholesalePlatform.Domain.Products;

namespace WholesalePlatform.Tests.Domain;

public sealed class ProductInvariantTests
{
    [Fact]
    public void Create_rejects_empty_sku()
    {
        Assert.Throws<DomainException>(() =>
            Product.Create("", "Product", null, 10m));
    }

    [Fact]
    public void Create_rejects_non_positive_price()
    {
        Assert.Throws<DomainException>(() =>
            Product.Create("SKU-1", "Product", null, 0m));
    }
}
