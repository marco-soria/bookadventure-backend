using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookAdventure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBookIsAvailableAndRentalOrderDetailProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                schema: "BookAdventure",
                table: "RentalOrderDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsReturned",
                schema: "BookAdventure",
                table: "RentalOrderDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnDate",
                schema: "BookAdventure",
                table: "RentalOrderDetail",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                schema: "BookAdventure",
                table: "Book",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrderDetail_DueDate",
                schema: "BookAdventure",
                table: "RentalOrderDetail",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrderDetail_IsReturned",
                schema: "BookAdventure",
                table: "RentalOrderDetail",
                column: "IsReturned");

            migrationBuilder.CreateIndex(
                name: "IX_Book_IsAvailable",
                schema: "BookAdventure",
                table: "Book",
                column: "IsAvailable");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RentalOrderDetail_DueDate",
                schema: "BookAdventure",
                table: "RentalOrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_RentalOrderDetail_IsReturned",
                schema: "BookAdventure",
                table: "RentalOrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_Book_IsAvailable",
                schema: "BookAdventure",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "DueDate",
                schema: "BookAdventure",
                table: "RentalOrderDetail");

            migrationBuilder.DropColumn(
                name: "IsReturned",
                schema: "BookAdventure",
                table: "RentalOrderDetail");

            migrationBuilder.DropColumn(
                name: "ReturnDate",
                schema: "BookAdventure",
                table: "RentalOrderDetail");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                schema: "BookAdventure",
                table: "Book");
        }
    }
}
