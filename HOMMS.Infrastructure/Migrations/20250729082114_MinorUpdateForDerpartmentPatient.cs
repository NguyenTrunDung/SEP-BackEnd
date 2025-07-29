using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HOMMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MinorUpdateForDerpartmentPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "departmentId",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Department",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_departmentId",
                table: "Patients",
                column: "departmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_LocationId",
                table: "Department",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Department_Locations_LocationId",
                table: "Department",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Department_departmentId",
                table: "Patients",
                column: "departmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Department_Locations_LocationId",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Department_departmentId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_departmentId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Department_LocationId",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "departmentId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Department");
        }
    }
}
