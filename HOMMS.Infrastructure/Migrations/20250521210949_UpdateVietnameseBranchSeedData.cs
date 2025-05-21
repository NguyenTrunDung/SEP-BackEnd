using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HOMMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVietnameseBranchSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 5,
                column: "Address",
                value: "Coteccons");

            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Address", "Name" },
                values: new object[] { "Cần Thơ", "Bệnh viện Hoàn Mỹ Cửu Long Canteen" });

            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Address", "Name" },
                values: new object[] { "TP Hồ Chí Minh", "BV Nhi Đồng Canteen" });

            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 14,
                column: "Address",
                value: "Đại lộ Bình Dương, khu Gò Cát, Lái Thiêu, Thuận An, Bình Dương");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 5,
                column: "Address",
                value: "Conteccons");

            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Address", "Name" },
                values: new object[] { "C?n Th?", "B?nh vi?n Hoàn M? C?u Long Canteen" });

            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Address", "Name" },
                values: new object[] { "TP H? Chí Minh", "BV Nhi ??ng Canteen" });

            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 14,
                column: "Address",
                value: "??i l? Bình D??ng, khu Gò Cát, Lái Thiêu, Thu?n An, Bình D??ng");
        }
    }
}
