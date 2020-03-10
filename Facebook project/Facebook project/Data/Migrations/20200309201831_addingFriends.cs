using Microsoft.EntityFrameworkCore.Migrations;

namespace Facebook_project.Data.Migrations
{
    public partial class addingFriends : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Friends",
                columns: table => new
                {
                    senderUserID = table.Column<string>(nullable: false),
                    receiverUserID = table.Column<string>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friends", x => new { x.senderUserID, x.receiverUserID });
                    table.ForeignKey(
                        name: "FK_Friends_AspNetUsers_receiverUserID",
                        column: x => x.receiverUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Friends_AspNetUsers_senderUserID",
                        column: x => x.senderUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Friends_receiverUserID",
                table: "Friends",
                column: "receiverUserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friends");
        }
    }
}
