using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Forno.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 240, nullable: false),
                    Total = table.Column<decimal>(type: "TEXT", precision: 8, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 24, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pizzas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Tagline = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 480, nullable: false),
                    Ingredients = table.Column<string>(type: "TEXT", maxLength: 240, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: false),
                    Tone = table.Column<string>(type: "TEXT", maxLength: 24, nullable: false),
                    Featured = table.Column<bool>(type: "INTEGER", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pizzas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "subscribers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscribers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "order_lines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    PizzaId = table.Column<int>(type: "INTEGER", nullable: false),
                    PizzaSlug = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    PizzaName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_lines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_lines_orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_order_lines_OrderId",
                table: "order_lines",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_pizzas_Slug",
                table: "pizzas",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subscribers_Email",
                table: "subscribers",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_lines");

            migrationBuilder.DropTable(
                name: "pizzas");

            migrationBuilder.DropTable(
                name: "subscribers");

            migrationBuilder.DropTable(
                name: "orders");
        }
    }
}
