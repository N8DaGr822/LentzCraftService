using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LentzCraftServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductDisplayOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Initialize existing products with sequential display order
            // based on their current CreatedDate ordering (newest first gets lowest number)
            migrationBuilder.Sql(@"
                WITH Ordered AS (
                    SELECT Id, ROW_NUMBER() OVER (ORDER BY CreatedDate DESC) AS RowNum
                    FROM Products
                )
                UPDATE Products
                SET DisplayOrder = Ordered.RowNum
                FROM Products
                INNER JOIN Ordered ON Products.Id = Ordered.Id
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Products_DisplayOrder",
                table: "Products",
                column: "DisplayOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_DisplayOrder",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Products");
        }
    }
}
