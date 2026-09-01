using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Forno.Data.Migrations
{
    /// <inheritdoc />
    public partial class ShopSettingsAndFulfillment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Fulfillment",
                table: "orders",
                type: "TEXT",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "oven_settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", maxLength: 48, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oven_settings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_oven_settings_Key",
                table: "oven_settings",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "oven_settings");

            migrationBuilder.DropColumn(
                name: "Fulfillment",
                table: "orders");
        }
    }
}
