using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WholesalePlatform.Domain.Orders;
using WholesalePlatform.Domain.Products;

namespace WholesalePlatform.Infrastructure.Persistence.Configurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable(
            "order_items",
            table =>
            {
                table.HasCheckConstraint("ck_order_items_unit_price_positive", "\"UnitPrice\" > 0");
                table.HasCheckConstraint("ck_order_items_quantity_positive", "\"Quantity\" > 0");
                table.HasCheckConstraint("ck_order_items_line_total_matches", "\"LineTotal\" = \"UnitPrice\" * \"Quantity\"");
            });

        builder.HasKey(item => item.Id);

        builder.Property(item => item.ProductSku)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(item => item.ProductName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(item => item.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(item => item.LineTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(item => item.Version)
            .IsConcurrencyToken()
            .IsRequired();

        builder.HasQueryFilter(item => !item.IsDeleted);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
