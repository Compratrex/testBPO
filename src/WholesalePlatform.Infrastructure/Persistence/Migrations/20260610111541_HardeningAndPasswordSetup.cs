using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WholesalePlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class HardeningAndPasswordSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PasswordSetupTokenExpiresAt",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordSetupTokenHash",
                table: "users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PermissionVersion",
                table: "users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "order_items",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "order_items",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "order_items",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "order_items",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "order_items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModifiedAt",
                table: "order_items",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "order_items",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "order_items",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordSetupTokenExpiresAt",
                table: "users");

            migrationBuilder.DropColumn(
                name: "PasswordSetupTokenHash",
                table: "users");

            migrationBuilder.DropColumn(
                name: "PermissionVersion",
                table: "users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "order_items");
        }
    }
}
