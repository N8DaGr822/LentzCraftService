using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LentzCraftServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductModifiedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Products') AND name = 'ModifiedDate')
                    ALTER TABLE [Products] ADD [ModifiedDate] datetime2 NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Products");
        }
    }
}
