using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dress_Up.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdaugareAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clothes_Outfit_OutfitId",
                table: "Clothes");

            migrationBuilder.DropForeignKey(
                name: "FK_Outfit_AspNetUsers_UserId",
                table: "Outfit");

            migrationBuilder.DropForeignKey(
                name: "FK_UserEvent_Outfit_OutfitId",
                table: "UserEvent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Outfit",
                table: "Outfit");

            migrationBuilder.RenameTable(
                name: "Outfit",
                newName: "Outfits");

            migrationBuilder.RenameIndex(
                name: "IX_Outfit_UserId",
                table: "Outfits",
                newName: "IX_Outfits_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Outfits",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Outfits",
                table: "Outfits",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Avatars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageData = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avatars", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Clothes_Outfits_OutfitId",
                table: "Clothes",
                column: "OutfitId",
                principalTable: "Outfits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Outfits_AspNetUsers_UserId",
                table: "Outfits",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserEvent_Outfits_OutfitId",
                table: "UserEvent",
                column: "OutfitId",
                principalTable: "Outfits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clothes_Outfits_OutfitId",
                table: "Clothes");

            migrationBuilder.DropForeignKey(
                name: "FK_Outfits_AspNetUsers_UserId",
                table: "Outfits");

            migrationBuilder.DropForeignKey(
                name: "FK_UserEvent_Outfits_OutfitId",
                table: "UserEvent");

            migrationBuilder.DropTable(
                name: "Avatars");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Outfits",
                table: "Outfits");

            migrationBuilder.RenameTable(
                name: "Outfits",
                newName: "Outfit");

            migrationBuilder.RenameIndex(
                name: "IX_Outfits_UserId",
                table: "Outfit",
                newName: "IX_Outfit_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Outfit",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Outfit",
                table: "Outfit",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Clothes_Outfit_OutfitId",
                table: "Clothes",
                column: "OutfitId",
                principalTable: "Outfit",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Outfit_AspNetUsers_UserId",
                table: "Outfit",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserEvent_Outfit_OutfitId",
                table: "UserEvent",
                column: "OutfitId",
                principalTable: "Outfit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
