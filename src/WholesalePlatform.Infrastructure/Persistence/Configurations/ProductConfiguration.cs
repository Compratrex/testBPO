using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WholesalePlatform.Domain.Products;

namespace WholesalePlatform.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(
            "products",
            table => table.HasCheckConstraint("ck_products_price_positive", "\"Price\" > 0"));

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Sku)
            .HasMaxLength(Product.MaxSkuLength)
            .IsRequired();

        builder.Property(product => product.Name)
            .HasMaxLength(Product.MaxNameLength)
            .IsRequired();

        builder.Property(product => product.Description)
            .HasMaxLength(Product.MaxDescriptionLength);

        builder.Property(product => product.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(product => product.Version)
            .IsConcurrencyToken()
            .IsRequired();

        builder.HasIndex(product => product.Sku)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
        builder.HasQueryFilter(product => !product.IsDeleted);
    }
}
