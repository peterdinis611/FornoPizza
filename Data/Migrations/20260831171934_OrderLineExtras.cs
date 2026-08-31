using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Forno.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrderLineExtras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Extras",
                table: "order_lines",
                type: "TEXT",
                maxLength: 240,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Extras",
                table: "order_lines");
        }
    }
}
