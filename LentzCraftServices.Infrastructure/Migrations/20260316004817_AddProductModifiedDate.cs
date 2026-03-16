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
            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Products",
                type: "datetime2",
                nullable: true);
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
