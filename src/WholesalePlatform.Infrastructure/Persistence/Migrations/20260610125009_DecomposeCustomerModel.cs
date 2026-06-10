using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WholesalePlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DecomposeCustomerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_users_CustomerId",
                table: "orders");

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LegalAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "customer_users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPrimaryContact = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_customer_users_customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_customer_users_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql(
                """
                INSERT INTO customers (
                    "Id",
                    "Name",
                    "LegalAddress",
                    "CreatedAt",
                    "CreatedBy",
                    "LastModifiedAt",
                    "LastModifiedBy",
                    "IsDeleted",
                    "DeletedAt",
                    "DeletedBy",
                    "Version")
                SELECT
                    "Id",
                    "FullName",
                    "LegalAddress",
                    "CreatedAt",
                    "CreatedBy",
                    "LastModifiedAt",
                    "LastModifiedBy",
                    "IsDeleted",
                    "DeletedAt",
                    "DeletedBy",
                    "Version"
                FROM users
                WHERE "Role" = 'Customer';
                """);

            migrationBuilder.Sql(
                """
                INSERT INTO customer_users ("Id", "CustomerId", "UserId", "IsPrimaryContact")
                SELECT "Id", "Id", "Id", TRUE
                FROM users
                WHERE "Role" = 'Customer';
                """);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "orders",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "orders",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LineTotal",
                table: "order_items",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ProductSku",
                table: "order_items",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                """
                UPDATE orders
                SET "CreatedByUserId" = "CustomerId",
                    "Currency" = 'USD';
                """);

            migrationBuilder.Sql(
                """
                UPDATE order_items AS item
                SET "LineTotal" = item."UnitPrice" * item."Quantity",
                    "ProductSku" = COALESCE(product."Sku", item."ProductId"::text)
                FROM products AS product
                WHERE product."Id" = item."ProductId";
                """);

            migrationBuilder.Sql(
                """
                UPDATE orders AS target
                SET "TotalAmount" = COALESCE(source."TotalAmount", 0)
                FROM (
                    SELECT "OrderId", SUM("LineTotal") AS "TotalAmount"
                    FROM order_items
                    GROUP BY "OrderId"
                ) AS source
                WHERE source."OrderId" = target."Id";
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedByUserId",
                table: "orders",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "LegalAddress",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "IX_orders_CreatedByUserId",
                table: "orders",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_ProductId",
                table: "order_items",
                column: "ProductId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_order_items_products_ProductId",
                table: "order_items",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_customers_CustomerId",
                table: "orders",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_users_CreatedByUserId",
                table: "orders",
                column: "CreatedByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_items_products_ProductId",
                table: "order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_customers_CustomerId",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_users_CreatedByUserId",
                table: "orders");

            migrationBuilder.DropTable(
                name: "customer_users");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropIndex(
                name: "IX_orders_CreatedByUserId",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_order_items_ProductId",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "LineTotal",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "ProductSku",
                table: "order_items");

            migrationBuilder.AddColumn<string>(
                name: "LegalAddress",
                table: "users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_users_CustomerId",
                table: "orders",
                column: "CustomerId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
