using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

public partial class Init : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AspNetRoles",
            columns: table => new {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                NormalizedName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table => {
                table.PrimaryKey("PK_AspNetRoles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUsers",
            columns: table => new {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FullName_FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                FullName_LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                AccessFailedCount = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_AspNetUsers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Brands",
            columns: table => new {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_Brands", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Categories",
            columns: table => new {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                DescriptionEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                DescriptionAr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                ImageKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_Categories", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Discounts",
            columns: table => new {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                DiscountType = table.Column<int>(type: "int", nullable: false),
                Value = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                ValidityPeriod_StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                ValidityPeriod_EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_Discounts", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "NewsletterSubscribers",
            columns: table => new {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                IsSubscribed = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_NewsletterSubscribers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PromoCodes",
            columns: table => new {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                DiscountType = table.Column<int>(type: "int", nullable: false),
                Value = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                MinimumOrder = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                UsageLimit = table.Column<int>(type: "int", nullable: true),
                UsedCount = table.Column<int>(type: "int", nullable: false),
                ValidityPeriod_StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                ValidityPeriod_EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_PromoCodes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Suppliers",
            columns: table => new {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                ContactName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                TaxNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table => {
                table.PrimaryKey("PK_Suppliers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AspNetRoleClaims",
            columns: table => new {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table => {
                table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserClaims",
            columns: table => new {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table => {
                table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserLogins",
            columns: table => new {
                LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                table.ForeignKey(
                    name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserRoles",
            columns: table => new {
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                table.ForeignKey(
                    name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserTokens",
            columns: table => new {
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table => {
                table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                table.ForeignKey(
                    name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Carts",
            columns: table => new {
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_Carts", x => x.UserId);
                table.ForeignKey(
                    name: "FK_Carts_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "RefreshTokens",
            columns: table => new {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Token = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                table.ForeignKey(
                    name: "FK_RefreshTokens_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Products",
            columns: table => new {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                SKU = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                DescriptionEn = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                DescriptionAr = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DiscountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_Products", x => x.Id);
                table.ForeignKey(
                    name: "FK_Products_Brands_BrandId",
                    column: x => x.BrandId,
                    principalTable: "Brands",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Products_Categories_CategoryId",
                    column: x => x.CategoryId,
                    principalTable: "Categories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Products_Discounts_DiscountId",
                    column: x => x.DiscountId,
                    principalTable: "Discounts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "Orders",
            columns: table => new {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                Subtotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                ShippingFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                ItemsDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                PromoDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                ShippingAddress_Street = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                ShippingAddress_City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                ShippingAddress_Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                ShippingAddress_Notes = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                PromoCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_Orders", x => x.Id);
                table.ForeignKey(
                    name: "FK_Orders_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Orders_PromoCodes_PromoCodeId",
                    column: x => x.PromoCodeId,
                    principalTable: "PromoCodes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "PurchaseOrders",
            columns: table => new {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                OrderDate = table.Column<DateOnly>(type: "date", nullable: false),
                ExpectedDeliveryDate = table.Column<DateOnly>(type: "date", nullable: true),
                Subtotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                ShippingCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                table.ForeignKey(
                    name: "FK_PurchaseOrders_Suppliers_SupplierId",
                    column: x => x.SupplierId,
                    principalTable: "Suppliers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "CartItems",
            columns: table => new {
                CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Quantity = table.Column<int>(type: "int", nullable: false),
                UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_CartItems", x => new { x.CartId, x.ProductId });
                table.ForeignKey(
                    name: "FK_CartItems_Carts_CartId",
                    column: x => x.CartId,
                    principalTable: "Carts",
                    principalColumn: "UserId",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CartItems_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Inventories",
            columns: table => new {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                QuantityOnHand = table.Column<int>(type: "int", nullable: false),
                ReorderLevel = table.Column<int>(type: "int", nullable: false),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_Inventories", x => x.Id);
                table.ForeignKey(
                    name: "FK_Inventories_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "ProductImages",
            columns: table => new {
                ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ImageKey = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_ProductImages", x => new { x.ProductId, x.ImageKey });
                table.ForeignKey(
                    name: "FK_ProductImages_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Reviews",
            columns: table => new {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Rating = table.Column<int>(type: "int", nullable: false),
                Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_Reviews", x => x.Id);
                table.ForeignKey(
                    name: "FK_Reviews_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Reviews_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "OrderItems",
            columns: table => new {
                OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Quantity = table.Column<int>(type: "int", nullable: false),
                UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_OrderItems", x => new { x.OrderId, x.ProductId });
                table.ForeignKey(
                    name: "FK_OrderItems_Orders_OrderId",
                    column: x => x.OrderId,
                    principalTable: "Orders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_OrderItems_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Payments",
            columns: table => new {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                RefundedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_Payments", x => x.Id);
                table.ForeignKey(
                    name: "FK_Payments_Orders_OrderId",
                    column: x => x.OrderId,
                    principalTable: "Orders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "GoodsReceipts",
            columns: table => new {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                PurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeliveryReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table => {
                table.PrimaryKey("PK_GoodsReceipts", x => x.Id);
                table.ForeignKey(
                    name: "FK_GoodsReceipts_PurchaseOrders_PurchaseOrderId",
                    column: x => x.PurchaseOrderId,
                    principalTable: "PurchaseOrders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "PurchaseOrderItems",
            columns: table => new {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OrderedQuantity = table.Column<int>(type: "int", nullable: false),
                ReceivedQuantity = table.Column<int>(type: "int", nullable: false),
                UnitCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_PurchaseOrderItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_PurchaseOrderItems_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId",
                    column: x => x.PurchaseOrderId,
                    principalTable: "PurchaseOrders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "PaymentAttempts",
            columns: table => new {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Method = table.Column<int>(type: "int", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                GatewayResponse = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                FailedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table => {
                table.PrimaryKey("PK_PaymentAttempts", x => x.Id);
                table.ForeignKey(
                    name: "FK_PaymentAttempts_Payments_PaymentId",
                    column: x => x.PaymentId,
                    principalTable: "Payments",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "GoodsReceiptItems",
            columns: table => new {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                GoodsReceiptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Quantity = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_GoodsReceiptItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_GoodsReceiptItems_GoodsReceipts_GoodsReceiptId",
                    column: x => x.GoodsReceiptId,
                    principalTable: "GoodsReceipts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_GoodsReceiptItems_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "InventoryTransactions",
            columns: table => new {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                InventoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                QuantityChange = table.Column<int>(type: "int", nullable: false),
                QuantityBefore = table.Column<int>(type: "int", nullable: false),
                QuantityAfter = table.Column<int>(type: "int", nullable: false),
                OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                GoodsReceiptId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_InventoryTransactions", x => x.Id);
                table.ForeignKey(
                    name: "FK_InventoryTransactions_GoodsReceipts_GoodsReceiptId",
                    column: x => x.GoodsReceiptId,
                    principalTable: "GoodsReceipts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_InventoryTransactions_Inventories_InventoryId",
                    column: x => x.InventoryId,
                    principalTable: "Inventories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_InventoryTransactions_Orders_OrderId",
                    column: x => x.OrderId,
                    principalTable: "Orders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_AspNetRoleClaims_RoleId",
            table: "AspNetRoleClaims",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "RoleNameIndex",
            table: "AspNetRoles",
            column: "NormalizedName",
            unique: true,
            filter: "[NormalizedName] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserClaims_UserId",
            table: "AspNetUserClaims",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserLogins_UserId",
            table: "AspNetUserLogins",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserRoles_RoleId",
            table: "AspNetUserRoles",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "EmailIndex",
            table: "AspNetUsers",
            column: "NormalizedEmail");

        migrationBuilder.CreateIndex(
            name: "UserNameIndex",
            table: "AspNetUsers",
            column: "NormalizedUserName",
            unique: true,
            filter: "[NormalizedUserName] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Brands_NameAr",
            table: "Brands",
            column: "NameAr",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Brands_NameEn",
            table: "Brands",
            column: "NameEn",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_CartItems_ProductId",
            table: "CartItems",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "IX_GoodsReceiptItems_GoodsReceiptId_ProductId",
            table: "GoodsReceiptItems",
            columns: new[] { "GoodsReceiptId", "ProductId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_GoodsReceiptItems_ProductId",
            table: "GoodsReceiptItems",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "IX_GoodsReceipts_Number",
            table: "GoodsReceipts",
            column: "Number",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_GoodsReceipts_PurchaseOrderId",
            table: "GoodsReceipts",
            column: "PurchaseOrderId");

        migrationBuilder.CreateIndex(
            name: "IX_Inventories_ProductId",
            table: "Inventories",
            column: "ProductId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_InventoryTransactions_GoodsReceiptId",
            table: "InventoryTransactions",
            column: "GoodsReceiptId");

        migrationBuilder.CreateIndex(
            name: "IX_InventoryTransactions_InventoryId",
            table: "InventoryTransactions",
            column: "InventoryId");

        migrationBuilder.CreateIndex(
            name: "IX_InventoryTransactions_OrderId",
            table: "InventoryTransactions",
            column: "OrderId");

        migrationBuilder.CreateIndex(
            name: "IX_NewsletterSubscribers_Email",
            table: "NewsletterSubscribers",
            column: "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_OrderItems_ProductId",
            table: "OrderItems",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "IX_Orders_PromoCodeId",
            table: "Orders",
            column: "PromoCodeId");

        migrationBuilder.CreateIndex(
            name: "IX_Orders_UserId",
            table: "Orders",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_PaymentAttempts_PaymentId",
            table: "PaymentAttempts",
            column: "PaymentId");

        migrationBuilder.CreateIndex(
            name: "IX_Payments_OrderId",
            table: "Payments",
            column: "OrderId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Products_BrandId",
            table: "Products",
            column: "BrandId");

        migrationBuilder.CreateIndex(
            name: "IX_Products_CategoryId",
            table: "Products",
            column: "CategoryId");

        migrationBuilder.CreateIndex(
            name: "IX_Products_DiscountId",
            table: "Products",
            column: "DiscountId");

        migrationBuilder.CreateIndex(
            name: "IX_Products_SKU",
            table: "Products",
            column: "SKU",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_PromoCodes_Code",
            table: "PromoCodes",
            column: "Code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_PurchaseOrderItems_ProductId",
            table: "PurchaseOrderItems",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "IX_PurchaseOrderItems_PurchaseOrderId_ProductId",
            table: "PurchaseOrderItems",
            columns: new[] { "PurchaseOrderId", "ProductId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_PurchaseOrders_Number",
            table: "PurchaseOrders",
            column: "Number",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_PurchaseOrders_SupplierId",
            table: "PurchaseOrders",
            column: "SupplierId");

        migrationBuilder.CreateIndex(
            name: "IX_RefreshTokens_Token",
            table: "RefreshTokens",
            column: "Token",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_RefreshTokens_UserId",
            table: "RefreshTokens",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_Reviews_ProductId",
            table: "Reviews",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "IX_Reviews_UserId_ProductId",
            table: "Reviews",
            columns: new[] { "UserId", "ProductId" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AspNetRoleClaims");

        migrationBuilder.DropTable(
            name: "AspNetUserClaims");

        migrationBuilder.DropTable(
            name: "AspNetUserLogins");

        migrationBuilder.DropTable(
            name: "AspNetUserRoles");

        migrationBuilder.DropTable(
            name: "AspNetUserTokens");

        migrationBuilder.DropTable(
            name: "CartItems");

        migrationBuilder.DropTable(
            name: "GoodsReceiptItems");

        migrationBuilder.DropTable(
            name: "InventoryTransactions");

        migrationBuilder.DropTable(
            name: "NewsletterSubscribers");

        migrationBuilder.DropTable(
            name: "OrderItems");

        migrationBuilder.DropTable(
            name: "PaymentAttempts");

        migrationBuilder.DropTable(
            name: "ProductImages");

        migrationBuilder.DropTable(
            name: "PurchaseOrderItems");

        migrationBuilder.DropTable(
            name: "RefreshTokens");

        migrationBuilder.DropTable(
            name: "Reviews");

        migrationBuilder.DropTable(
            name: "AspNetRoles");

        migrationBuilder.DropTable(
            name: "Carts");

        migrationBuilder.DropTable(
            name: "GoodsReceipts");

        migrationBuilder.DropTable(
            name: "Inventories");

        migrationBuilder.DropTable(
            name: "Payments");

        migrationBuilder.DropTable(
            name: "PurchaseOrders");

        migrationBuilder.DropTable(
            name: "Products");

        migrationBuilder.DropTable(
            name: "Orders");

        migrationBuilder.DropTable(
            name: "Suppliers");

        migrationBuilder.DropTable(
            name: "Brands");

        migrationBuilder.DropTable(
            name: "Categories");

        migrationBuilder.DropTable(
            name: "Discounts");

        migrationBuilder.DropTable(
            name: "AspNetUsers");

        migrationBuilder.DropTable(
            name: "PromoCodes");
    }
}
