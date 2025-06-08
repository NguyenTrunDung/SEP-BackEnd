using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HOMMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PatientDiseaseTable_Fixrelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders");

            migrationBuilder.AddColumn<DateTime>(
                name: "AdmissionDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttendingPhysician",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BedNumber",
                table: "Users",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DischargeDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatientId",
                table: "Users",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresDietarySupervision",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RoomNumber",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPatientOrder",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PatientId",
                table: "Orders",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DiseaseCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DietaryRestrictions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RecommendedFoods = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SeverityLevel = table.Column<int>(type: "int", nullable: false),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: false),
                    ColorCode = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiseaseCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiseaseCategories_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    MedicalRecordNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    RoomNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BedNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    AdmissionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DischargeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttendingPhysician = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequiresDietarySupervision = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ExternalSystemId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastSyncAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patients_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiseaseCategoryFoodRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    DiseaseCategoryId = table.Column<int>(type: "int", nullable: false),
                    FoodId = table.Column<int>(type: "int", nullable: false),
                    RestrictionLevel = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AlternativeRecommendations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RequiresPhysicianOverride = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiseaseCategoryFoodRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiseaseCategoryFoodRestrictions_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiseaseCategoryFoodRestrictions_DiseaseCategories_DiseaseCategoryId",
                        column: x => x.DiseaseCategoryId,
                        principalTable: "DiseaseCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiseaseCategoryFoodRestrictions_Foods_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Foods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientDiseaseCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    DiseaseCategoryId = table.Column<int>(type: "int", nullable: false),
                    DiagnosedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PatientSeverityLevel = table.Column<int>(type: "int", nullable: true),
                    PatientSpecificNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AssignedByPhysician = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientDiseaseCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientDiseaseCategories_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientDiseaseCategories_DiseaseCategories_DiseaseCategoryId",
                        column: x => x.DiseaseCategoryId,
                        principalTable: "DiseaseCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientDiseaseCategories_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_PatientId",
                table: "Users",
                column: "PatientId",
                unique: true,
                filter: "[PatientId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IsPatientOrder",
                table: "Orders",
                column: "IsPatientOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PatientId",
                table: "Orders",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Foods_DiseaseCategoryId",
                table: "Foods",
                column: "DiseaseCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseCategories_BranchId",
                table: "DiseaseCategories",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseCategories_BranchId_Code",
                table: "DiseaseCategories",
                columns: new[] { "BranchId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseCategories_IsActive",
                table: "DiseaseCategories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseCategoryFoodRestrictions_BranchId",
                table: "DiseaseCategoryFoodRestrictions",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseCategoryFoodRestrictions_DiseaseCategoryId_FoodId",
                table: "DiseaseCategoryFoodRestrictions",
                columns: new[] { "DiseaseCategoryId", "FoodId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseCategoryFoodRestrictions_FoodId",
                table: "DiseaseCategoryFoodRestrictions",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseCategoryFoodRestrictions_IsActive",
                table: "DiseaseCategoryFoodRestrictions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseCategoryFoodRestrictions_RestrictionLevel",
                table: "DiseaseCategoryFoodRestrictions",
                column: "RestrictionLevel");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDiseaseCategories_BranchId",
                table: "PatientDiseaseCategories",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDiseaseCategories_DiseaseCategoryId",
                table: "PatientDiseaseCategories",
                column: "DiseaseCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDiseaseCategories_ExpiryDate",
                table: "PatientDiseaseCategories",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDiseaseCategories_IsActive",
                table: "PatientDiseaseCategories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDiseaseCategories_PatientId_DiseaseCategoryId",
                table: "PatientDiseaseCategories",
                columns: new[] { "PatientId", "DiseaseCategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_BranchId",
                table: "Patients",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_BranchId_MedicalRecordNumber",
                table: "Patients",
                columns: new[] { "BranchId", "MedicalRecordNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_BranchId_Room_Bed",
                table: "Patients",
                columns: new[] { "BranchId", "RoomNumber", "BedNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_ExternalSystemId",
                table: "Patients",
                column: "ExternalSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_IsActive",
                table: "Patients",
                column: "IsActive");

            migrationBuilder.AddForeignKey(
                name: "FK_Foods_DiseaseCategories_DiseaseCategoryId",
                table: "Foods",
                column: "DiseaseCategoryId",
                principalTable: "DiseaseCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Patients_PatientId",
                table: "Orders",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Patients_PatientId",
                table: "Users",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Foods_DiseaseCategories_DiseaseCategoryId",
                table: "Foods");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Patients_PatientId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Patients_PatientId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "DiseaseCategoryFoodRestrictions");

            migrationBuilder.DropTable(
                name: "PatientDiseaseCategories");

            migrationBuilder.DropTable(
                name: "DiseaseCategories");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Users_PatientId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Orders_IsPatientOrder",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PatientId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Foods_DiseaseCategoryId",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "AdmissionDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AttendingPhysician",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BedNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DischargeDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RequiresDietarySupervision",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RoomNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsPatientOrder",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
