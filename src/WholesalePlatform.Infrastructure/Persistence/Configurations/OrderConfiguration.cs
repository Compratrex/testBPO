using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WholesalePlatform.Domain.Customers;
using WholesalePlatform.Domain.Enums;
using WholesalePlatform.Domain.Orders;
using WholesalePlatform.Domain.Users;

namespace WholesalePlatform.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        var statuses = string.Join(", ", Enum.GetNames<OrderStatus>().Select(status => $"'{status}'"));

        builder.ToTable(
            "orders",
            table =>
            {
                table.HasCheckConstraint("ck_orders_status_valid", $"\"Status\" IN ({statuses})");
                table.HasCheckConstraint("ck_orders_total_amount_non_negative", "\"TotalAmount\" >= 0");
                table.HasCheckConstraint(
                    "ck_orders_currency_format",
                    "char_length(\"Currency\") = 3 AND \"Currency\" = upper(\"Currency\")");
            });

        builder.HasKey(order => order.Id);

        builder.Property(order => order.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(order => order.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(order => order.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(order => order.Version)
            .IsConcurrencyToken()
            .IsRequired();

        builder.HasQueryFilter(order => !order.IsDeleted);

        builder.HasMany(order => order.Items)
            .WithOne()
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(order => order.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(order => order.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(order => order.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
