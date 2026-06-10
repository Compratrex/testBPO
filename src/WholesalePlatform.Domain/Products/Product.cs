using WholesalePlatform.Domain.Common;

namespace WholesalePlatform.Domain.Products;

public sealed class Product : AuditableEntity
{
    public const int MaxSkuLength = 64;
    public const int MaxNameLength = 200;
    public const int MaxDescriptionLength = 1000;

    private Product()
    {
    }

    private Product(Guid id, string sku, string name, string? description, decimal price)
        : base(id)
    {
        Sku = sku;
        Name = name;
        Description = description;
        Price = price;
        IsAvailable = true;
    }

    public string Sku { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public bool IsAvailable { get; private set; }

    public static Product Create(string sku, string name, string? description, decimal price)
    {
        ValidateSku(sku);
        ValidateName(name);
        ValidateDescription(description);
        ValidatePrice(price);

        return new Product(Guid.NewGuid(), NormalizeSku(sku), name.Trim(), NormalizeDescription(description), price);
    }

    public void Update(string sku, string name, string? description, decimal price, bool isAvailable)
    {
        ValidateSku(sku);
        ValidateName(name);
        ValidateDescription(description);
        ValidatePrice(price);

        Sku = NormalizeSku(sku);
        Name = name.Trim();
        Description = NormalizeDescription(description);
        Price = price;
        IsAvailable = isAvailable;
    }

    private static void ValidatePrice(decimal price)
    {
        if (price <= 0)
        {
            throw new DomainException("Product price must be greater than zero.");
        }
    }

    public static string NormalizeSku(string sku)
    {
        return sku.Trim().ToUpperInvariant();
    }

    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    private static void ValidateSku(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new DomainException("Product SKU cannot be empty.");
        }

        if (NormalizeSku(sku).Length > MaxSkuLength)
        {
            throw new DomainException($"Product SKU cannot exceed {MaxSkuLength} characters.");
        }
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Product name cannot be empty.");
        }

        if (name.Trim().Length > MaxNameLength)
        {
            throw new DomainException($"Product name cannot exceed {MaxNameLength} characters.");
        }
    }

    private static void ValidateDescription(string? description)
    {
        if (NormalizeDescription(description)?.Length > MaxDescriptionLength)
        {
            throw new DomainException($"Product description cannot exceed {MaxDescriptionLength} characters.");
        }
    }
}
