using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WholesalePlatform.Domain.Enums;
using WholesalePlatform.Domain.Users;

namespace WholesalePlatform.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        var roles = string.Join(", ", Enum.GetNames<UserRole>().Select(role => $"'{role}'"));

        builder.ToTable(
            "users",
            table => table.HasCheckConstraint("ck_users_role_valid", $"\"Role\" IN ({roles})"));

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Email)
            .HasMaxLength(User.MaxEmailLength)
            .IsRequired();

        builder.Property(user => user.FullName)
            .HasMaxLength(User.MaxFullNameLength)
            .IsRequired();

        builder.Property(user => user.PasswordHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(user => user.PermissionVersion)
            .IsRequired();

        builder.Property(user => user.PasswordSetupTokenHash)
            .HasMaxLength(500);

        builder.Property(user => user.PasswordSetupTokenExpiresAt);

        builder.Property(user => user.Role)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(user => user.Version)
            .IsConcurrencyToken()
            .IsRequired();

        builder.HasIndex(user => user.Email)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
        builder.HasQueryFilter(user => !user.IsDeleted);

        builder.HasMany(user => user.Permissions)
            .WithOne()
            .HasForeignKey(permission => permission.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(user => user.Permissions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
