using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WholesalePlatform.Domain.Enums;
using WholesalePlatform.Domain.Permissions;

namespace WholesalePlatform.Infrastructure.Persistence.Configurations;

public sealed class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermission>
{
    public void Configure(EntityTypeBuilder<UserPermission> builder)
    {
        var permissions = string.Join(", ", Enum.GetNames<PermissionCode>().Select(permission => $"'{permission}'"));

        builder.ToTable(
            "user_permissions",
            table => table.HasCheckConstraint(
                "ck_user_permissions_permission_valid",
                $"\"Permission\" IN ({permissions})"));

        builder.HasKey(permission => permission.Id);

        builder.Property(permission => permission.Permission)
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(permission => permission.Version)
            .IsConcurrencyToken()
            .IsRequired();

        builder.HasIndex(permission => new { permission.UserId, permission.Permission })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasQueryFilter(permission => !permission.IsDeleted);
    }
}
