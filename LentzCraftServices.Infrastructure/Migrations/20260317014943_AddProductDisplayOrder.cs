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
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Products') AND name = 'DisplayOrder')
                    ALTER TABLE [Products] ADD [DisplayOrder] int NOT NULL CONSTRAINT DF_Products_DisplayOrder DEFAULT 0;
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Products_DisplayOrder' AND object_id = OBJECT_ID('Products'))
                    CREATE INDEX [IX_Products_DisplayOrder] ON [Products] ([DisplayOrder]);
            ");
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
