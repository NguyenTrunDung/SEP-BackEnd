using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HOMMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBranchUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BranchUser_Branches_BranchId",
                table: "BranchUser");

            migrationBuilder.DropForeignKey(
                name: "FK_BranchUser_Users_UserId",
                table: "BranchUser");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_BranchUser_BranchUserId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BranchUser",
                table: "BranchUser");

            migrationBuilder.DropIndex(
                name: "IX_BranchUser_BranchId",
                table: "BranchUser");

            migrationBuilder.RenameTable(
                name: "BranchUser",
                newName: "BranchUsers");

            migrationBuilder.RenameIndex(
                name: "IX_BranchUser_UserId",
                table: "BranchUsers",
                newName: "IX_BranchUsers_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "BranchUsers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "BranchUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "BranchUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "BranchUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "BranchUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BranchUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "BranchUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "BranchUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_BranchUsers_BranchId_UserId",
                table: "BranchUsers",
                columns: new[] { "BranchId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BranchUsers",
                table: "BranchUsers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BranchUsers_UserId_IsDefault",
                table: "BranchUsers",
                columns: new[] { "UserId", "IsDefault" });

            migrationBuilder.AddForeignKey(
                name: "FK_BranchUsers_Branches_BranchId",
                table: "BranchUsers",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BranchUsers_Users_UserId",
                table: "BranchUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_BranchUsers_BranchUserId",
                table: "Orders",
                column: "BranchUserId",
                principalTable: "BranchUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BranchUsers_Branches_BranchId",
                table: "BranchUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_BranchUsers_Users_UserId",
                table: "BranchUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_BranchUsers_BranchUserId",
                table: "Orders");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_BranchUsers_BranchId_UserId",
                table: "BranchUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BranchUsers",
                table: "BranchUsers");

            migrationBuilder.DropIndex(
                name: "IX_BranchUsers_UserId_IsDefault",
                table: "BranchUsers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "BranchUsers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "BranchUsers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "BranchUsers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "BranchUsers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BranchUsers");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "BranchUsers");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "BranchUsers");

            migrationBuilder.RenameTable(
                name: "BranchUsers",
                newName: "BranchUser");

            migrationBuilder.RenameIndex(
                name: "IX_BranchUsers_UserId",
                table: "BranchUser",
                newName: "IX_BranchUser_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "BranchUser",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BranchUser",
                table: "BranchUser",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BranchUser_BranchId",
                table: "BranchUser",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_BranchUser_Branches_BranchId",
                table: "BranchUser",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BranchUser_Users_UserId",
                table: "BranchUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_BranchUser_BranchUserId",
                table: "Orders",
                column: "BranchUserId",
                principalTable: "BranchUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
