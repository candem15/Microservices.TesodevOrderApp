using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderService.Migrations
{
    public partial class fix1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_CustomerId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Addresses",
                newName: "AddressesId");

            migrationBuilder.AddColumn<Guid>(
                name: "AddressesId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AddressesId",
                table: "Orders",
                column: "AddressesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_AddressesId",
                table: "Orders",
                column: "AddressesId",
                principalTable: "Addresses",
                principalColumn: "AddressesId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_AddressesId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_AddressesId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AddressesId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "AddressesId",
                table: "Addresses",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
