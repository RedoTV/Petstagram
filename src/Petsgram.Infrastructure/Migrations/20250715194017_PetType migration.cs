using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Petsgram.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class PetTypemigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PetPhoto_Pets_PetId",
                table: "PetPhoto");

            migrationBuilder.DropForeignKey(
                name: "FK_Pets_PetType_PetTypeId",
                table: "Pets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PetType",
                table: "PetType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PetPhoto",
                table: "PetPhoto");

            migrationBuilder.RenameTable(
                name: "PetType",
                newName: "PetTypes");

            migrationBuilder.RenameTable(
                name: "PetPhoto",
                newName: "PetPhotos");

            migrationBuilder.RenameIndex(
                name: "IX_PetPhoto_PetId",
                table: "PetPhotos",
                newName: "IX_PetPhotos_PetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PetTypes",
                table: "PetTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PetPhotos",
                table: "PetPhotos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PetPhotos_Pets_PetId",
                table: "PetPhotos",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_PetTypes_PetTypeId",
                table: "Pets",
                column: "PetTypeId",
                principalTable: "PetTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PetPhotos_Pets_PetId",
                table: "PetPhotos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pets_PetTypes_PetTypeId",
                table: "Pets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PetTypes",
                table: "PetTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PetPhotos",
                table: "PetPhotos");

            migrationBuilder.RenameTable(
                name: "PetTypes",
                newName: "PetType");

            migrationBuilder.RenameTable(
                name: "PetPhotos",
                newName: "PetPhoto");

            migrationBuilder.RenameIndex(
                name: "IX_PetPhotos_PetId",
                table: "PetPhoto",
                newName: "IX_PetPhoto_PetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PetType",
                table: "PetType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PetPhoto",
                table: "PetPhoto",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PetPhoto_Pets_PetId",
                table: "PetPhoto",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_PetType_PetTypeId",
                table: "Pets",
                column: "PetTypeId",
                principalTable: "PetType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
