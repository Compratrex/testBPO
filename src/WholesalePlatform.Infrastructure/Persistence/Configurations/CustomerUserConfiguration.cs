using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WholesalePlatform.Domain.Customers;
using WholesalePlatform.Domain.Users;

namespace WholesalePlatform.Infrastructure.Persistence.Configurations;

public sealed class CustomerUserConfiguration : IEntityTypeConfiguration<CustomerUser>
{
    public void Configure(EntityTypeBuilder<CustomerUser> builder)
    {
        builder.ToTable("customer_users");

        builder.HasKey(customerUser => customerUser.Id);

        builder.HasIndex(customerUser => new { customerUser.CustomerId, customerUser.UserId })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasIndex(customerUser => customerUser.UserId)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasIndex(customerUser => customerUser.CustomerId)
            .IsUnique()
            .HasFilter("\"IsPrimaryContact\" = true AND \"IsDeleted\" = false");

        builder.Property(customerUser => customerUser.Version)
            .IsConcurrencyToken()
            .IsRequired();

        builder.HasQueryFilter(customerUser => !customerUser.IsDeleted);

        builder.HasOne(customerUser => customerUser.User)
            .WithMany()
            .HasForeignKey(customerUser => customerUser.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
