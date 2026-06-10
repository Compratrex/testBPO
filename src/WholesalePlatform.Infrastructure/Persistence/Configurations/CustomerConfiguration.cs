using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WholesalePlatform.Domain.Customers;

namespace WholesalePlatform.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.Name)
            .HasMaxLength(Customer.MaxNameLength)
            .IsRequired();

        builder.Property(customer => customer.LegalAddress)
            .HasMaxLength(Customer.MaxLegalAddressLength)
            .IsRequired();

        builder.Property(customer => customer.Version)
            .IsConcurrencyToken()
            .IsRequired();

        builder.HasQueryFilter(customer => !customer.IsDeleted);

        builder.HasMany(customer => customer.Users)
            .WithOne()
            .HasForeignKey(customerUser => customerUser.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(customer => customer.Users)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
