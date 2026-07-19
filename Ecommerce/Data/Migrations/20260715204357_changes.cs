using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Migrations
{
    /// <inheritdoc />
    public partial class changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Products",
                newName: "NameEn");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Products",
                newName: "DescriptionEn");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Categories",
                newName: "NameEn");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "NameEn",
                table: "Products",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DescriptionEn",
                table: "Products",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "NameEn",
                table: "Categories",
                newName: "Name");
        }
    }
}
