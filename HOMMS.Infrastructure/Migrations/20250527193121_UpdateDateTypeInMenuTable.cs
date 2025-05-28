using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HOMMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDateTypeInMenuTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Drop the computed column
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Menus");

            // 2. Alter the Date column
            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Menus",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            // 3. Re-add the computed column
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Menus",
                type: "nvarchar(41)", // or appropriate length
                nullable: true,
                computedColumnSql: "CONCAT([TimeOfDay], ' - ', CONVERT(VARCHAR(10), [Date], 120))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Menus");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Menus",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Menus",
                type: "nvarchar(41)", // or appropriate length
                nullable: true,
                computedColumnSql: "CONCAT([TimeOfDay], ' - ', CONVERT(VARCHAR(10), [Date], 120))");
        }
    }
}
