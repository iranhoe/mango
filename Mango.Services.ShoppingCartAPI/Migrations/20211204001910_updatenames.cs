using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mango.Services.ShoppingCartAPI.Migrations
{
    public partial class updatenames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Details_Header_CartHeaderId",
                table: "Details");

            migrationBuilder.DropForeignKey(
                name: "FK_Details_Products_ProductId",
                table: "Details");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Header",
                table: "Header");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Details",
                table: "Details");

            migrationBuilder.RenameTable(
                name: "Header",
                newName: "CartHeader");

            migrationBuilder.RenameTable(
                name: "Details",
                newName: "CartDetails");

            migrationBuilder.RenameIndex(
                name: "IX_Details_ProductId",
                table: "CartDetails",
                newName: "IX_CartDetails_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Details_CartHeaderId",
                table: "CartDetails",
                newName: "IX_CartDetails_CartHeaderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartHeader",
                table: "CartHeader",
                column: "CartHeaderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartDetails",
                table: "CartDetails",
                column: "CartDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_CartHeader_CartHeaderId",
                table: "CartDetails",
                column: "CartHeaderId",
                principalTable: "CartHeader",
                principalColumn: "CartHeaderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_Products_ProductId",
                table: "CartDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_CartHeader_CartHeaderId",
                table: "CartDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_Products_ProductId",
                table: "CartDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartHeader",
                table: "CartHeader");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartDetails",
                table: "CartDetails");

            migrationBuilder.RenameTable(
                name: "CartHeader",
                newName: "Header");

            migrationBuilder.RenameTable(
                name: "CartDetails",
                newName: "Details");

            migrationBuilder.RenameIndex(
                name: "IX_CartDetails_ProductId",
                table: "Details",
                newName: "IX_Details_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_CartDetails_CartHeaderId",
                table: "Details",
                newName: "IX_Details_CartHeaderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Header",
                table: "Header",
                column: "CartHeaderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Details",
                table: "Details",
                column: "CartDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Details_Header_CartHeaderId",
                table: "Details",
                column: "CartHeaderId",
                principalTable: "Header",
                principalColumn: "CartHeaderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Details_Products_ProductId",
                table: "Details",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
