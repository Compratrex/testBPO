using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WholesalePlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReviewHardening : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_Email",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_user_permissions_UserId_Permission",
                table: "user_permissions");

            migrationBuilder.DropIndex(
                name: "IX_products_Sku",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_outbox_messages_ProcessedAt_OccurredAt",
                table: "outbox_messages");

            migrationBuilder.DropIndex(
                name: "IX_customer_users_CustomerId_UserId",
                table: "customer_users");

            migrationBuilder.DropIndex(
                name: "IX_customer_users_UserId",
                table: "customer_users");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "user_permissions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "user_permissions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "user_permissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "user_permissions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "user_permissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModifiedAt",
                table: "user_permissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "user_permissions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "user_permissions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "outbox_messages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "outbox_messages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "outbox_messages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "outbox_messages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "outbox_messages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModifiedAt",
                table: "outbox_messages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "outbox_messages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockExpiresAt",
                table: "outbox_messages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockedAt",
                table: "outbox_messages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LockedBy",
                table: "outbox_messages",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "outbox_messages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "customer_users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "customer_users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "customer_users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "customer_users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "customer_users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModifiedAt",
                table: "customer_users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "customer_users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "customer_users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.Sql(
                """
                UPDATE users
                SET "Email" = lower(trim("Email"));
                """);

            migrationBuilder.Sql(
                """
                UPDATE products
                SET "Sku" = upper(trim("Sku")),
                    "Description" = nullif(trim(coalesce("Description", '')), '');
                """);

            migrationBuilder.Sql(
                """
                UPDATE user_permissions AS permission
                SET "CreatedAt" = COALESCE(user_account."CreatedAt", now()),
                    "CreatedBy" = user_account."CreatedBy",
                    "LastModifiedAt" = COALESCE(user_account."LastModifiedAt", user_account."CreatedAt", now()),
                    "LastModifiedBy" = user_account."LastModifiedBy",
                    "Version" = 1
                FROM users AS user_account
                WHERE user_account."Id" = permission."UserId";
                """);

            migrationBuilder.Sql(
                """
                UPDATE outbox_messages
                SET "CreatedAt" = "OccurredAt",
                    "LastModifiedAt" = "OccurredAt",
                    "Version" = 1;
                """);

            migrationBuilder.Sql(
                """
                UPDATE customer_users AS customer_user
                SET "CreatedAt" = COALESCE(customer."CreatedAt", user_account."CreatedAt", now()),
                    "CreatedBy" = COALESCE(customer."CreatedBy", user_account."CreatedBy"),
                    "LastModifiedAt" = COALESCE(customer."LastModifiedAt", user_account."LastModifiedAt", customer."CreatedAt", user_account."CreatedAt", now()),
                    "LastModifiedBy" = COALESCE(customer."LastModifiedBy", user_account."LastModifiedBy"),
                    "Version" = 1
                FROM customers AS customer, users AS user_account
                WHERE customer."Id" = customer_user."CustomerId"
                    AND user_account."Id" = customer_user."UserId";
                """);

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.AddCheckConstraint(
                name: "ck_users_role_valid",
                table: "users",
                sql: "\"Role\" IN ('Administrator', 'Customer')");

            migrationBuilder.CreateIndex(
                name: "IX_user_permissions_UserId_Permission",
                table: "user_permissions",
                columns: new[] { "UserId", "Permission" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.AddCheckConstraint(
                name: "ck_user_permissions_permission_valid",
                table: "user_permissions",
                sql: "\"Permission\" IN ('CustomersRead', 'CustomersManage', 'ProductsRead', 'ProductsManage', 'OrdersReadOwn', 'OrdersCreate', 'OrdersCancelOwn')");

            migrationBuilder.CreateIndex(
                name: "IX_products_Sku",
                table: "products",
                column: "Sku",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_outbox_messages_LockExpiresAt",
                table: "outbox_messages",
                column: "LockExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_outbox_messages_ProcessedAt_RetryCount_OccurredAt",
                table: "outbox_messages",
                columns: new[] { "ProcessedAt", "RetryCount", "OccurredAt" });

            migrationBuilder.AddCheckConstraint(
                name: "ck_orders_currency_format",
                table: "orders",
                sql: "char_length(\"Currency\") = 3 AND \"Currency\" = upper(\"Currency\")");

            migrationBuilder.AddCheckConstraint(
                name: "ck_orders_status_valid",
                table: "orders",
                sql: "\"Status\" IN ('PendingPayment', 'Paid', 'Cancelled')");

            migrationBuilder.AddCheckConstraint(
                name: "ck_orders_total_amount_non_negative",
                table: "orders",
                sql: "\"TotalAmount\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_order_items_line_total_matches",
                table: "order_items",
                sql: "\"LineTotal\" = \"UnitPrice\" * \"Quantity\"");

            migrationBuilder.CreateIndex(
                name: "IX_customer_users_CustomerId",
                table: "customer_users",
                column: "CustomerId",
                unique: true,
                filter: "\"IsPrimaryContact\" = true AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_customer_users_CustomerId_UserId",
                table: "customer_users",
                columns: new[] { "CustomerId", "UserId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_customer_users_UserId",
                table: "customer_users",
                column: "UserId",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_Email",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "ck_users_role_valid",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_user_permissions_UserId_Permission",
                table: "user_permissions");

            migrationBuilder.DropCheckConstraint(
                name: "ck_user_permissions_permission_valid",
                table: "user_permissions");

            migrationBuilder.DropIndex(
                name: "IX_products_Sku",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_outbox_messages_LockExpiresAt",
                table: "outbox_messages");

            migrationBuilder.DropIndex(
                name: "IX_outbox_messages_ProcessedAt_RetryCount_OccurredAt",
                table: "outbox_messages");

            migrationBuilder.DropCheckConstraint(
                name: "ck_orders_currency_format",
                table: "orders");

            migrationBuilder.DropCheckConstraint(
                name: "ck_orders_status_valid",
                table: "orders");

            migrationBuilder.DropCheckConstraint(
                name: "ck_orders_total_amount_non_negative",
                table: "orders");

            migrationBuilder.DropCheckConstraint(
                name: "ck_order_items_line_total_matches",
                table: "order_items");

            migrationBuilder.DropIndex(
                name: "IX_customer_users_CustomerId",
                table: "customer_users");

            migrationBuilder.DropIndex(
                name: "IX_customer_users_CustomerId_UserId",
                table: "customer_users");

            migrationBuilder.DropIndex(
                name: "IX_customer_users_UserId",
                table: "customer_users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "user_permissions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "user_permissions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "user_permissions");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "user_permissions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "user_permissions");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "user_permissions");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "user_permissions");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "user_permissions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "LockExpiresAt",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "LockedAt",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "LockedBy",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "customer_users");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "customer_users");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "customer_users");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "customer_users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "customer_users");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "customer_users");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "customer_users");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "customer_users");

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_permissions_UserId_Permission",
                table: "user_permissions",
                columns: new[] { "UserId", "Permission" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_products_Sku",
                table: "products",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_outbox_messages_ProcessedAt_OccurredAt",
                table: "outbox_messages",
                columns: new[] { "ProcessedAt", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_customer_users_CustomerId_UserId",
                table: "customer_users",
                columns: new[] { "CustomerId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_customer_users_UserId",
                table: "customer_users",
                column: "UserId",
                unique: true);
        }
    }
}
