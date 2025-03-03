using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class sampletext1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ItemCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Car Part" },
                    { 2, "Fastener" },
                    { 3, "Fluid" }
                });

            migrationBuilder.InsertData(
                table: "ItemLocation",
                columns: new[] { "Id", "Shelf", "Van" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 1 },
                    { 3, 1, 2 },
                    { 4, 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "CategoryId", "LocationId", "Name", "OptimalQuantity", "Price", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, 1, "Gearbox", 10, 1500.0, 5 },
                    { 2, 1, 2, "Clutch Plate", 15, 200.0, 8 },
                    { 3, 1, 3, "Brake Pad", 30, 50.0, 20 },
                    { 4, 3, 4, "Engine Oil", 40, 15.0, 25 },
                    { 5, 2, 1, "Bolt M10x50", 200, 0.5, 100 },
                    { 6, 2, 2, "Nut M10", 250, 0.29999999999999999, 150 },
                    { 7, 1, 3, "Oil Filter", 20, 20.0, 10 },
                    { 8, 1, 4, "Timing Belt", 12, 120.0, 6 },
                    { 9, 1, 1, "Radiator Hose", 20, 35.0, 12 },
                    { 10, 1, 2, "Spark Plug", 60, 10.0, 40 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ItemCategories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ItemCategories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ItemCategories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ItemLocation",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ItemLocation",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ItemLocation",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ItemLocation",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
