using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCancelledToSaleItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SaleNumber",
                table: "Sales",
                newName: "Id");

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "SaleItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "SaleItems");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Sales",
                newName: "SaleNumber");
        }
    }
}
