using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HOMMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAssignedByPhysicianField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PatientDiseaseCategories_ExpiryDate",
                table: "PatientDiseaseCategories");

            migrationBuilder.DropIndex(
                name: "IX_PatientDiseaseCategories_IsActive",
                table: "PatientDiseaseCategories");

            migrationBuilder.DropColumn(
                name: "AssignedByPhysician",
                table: "PatientDiseaseCategories");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "PatientDiseaseCategories",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientDiseaseCategories_CreatedBy",
                table: "PatientDiseaseCategories",
                column: "CreatedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PatientDiseaseCategories_CreatedBy",
                table: "PatientDiseaseCategories");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "PatientDiseaseCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedByPhysician",
                table: "PatientDiseaseCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientDiseaseCategories_ExpiryDate",
                table: "PatientDiseaseCategories",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDiseaseCategories_IsActive",
                table: "PatientDiseaseCategories",
                column: "IsActive");
        }
    }
}
