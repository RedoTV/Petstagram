using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Petsgram.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class CurrencyMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Pets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Pets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Pets");
        }
    }
}
