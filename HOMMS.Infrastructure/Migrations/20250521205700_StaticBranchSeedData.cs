using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HOMMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StaticBranchSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Branches",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.InsertData(
                table: "Branches",
                columns: new[] { "Id", "Address", "BranchId", "Code", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "Email", "IsActive", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "ManagerId", "Name", "Phone" },
                values: new object[,]
                {
                    { 5, "Conteccons", 5, "coteccons", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, true, false, null, null, null, "Coteccons", "0919000000" },
                    { 12, "C?n Th?", 12, "cthoanmy", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, true, false, null, null, null, "B?nh vi?n Hoàn M? C?u Long Canteen", "0123456789" },
                    { 13, "TP H? Chí Minh", 13, "bvnhi", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, true, false, null, null, null, "BV Nhi ??ng Canteen", "0123456789" },
                    { 14, "??i l? Bình D??ng, khu Gò Cát, Lái Thiêu, Thu?n An, Bình D??ng", 14, "becamex", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, true, false, null, null, null, "Becamex", "0919111111" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Branches_Code",
                table: "Branches",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Branches_Code",
                table: "Branches");

            migrationBuilder.DeleteData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Branches",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);
        }
    }
}
