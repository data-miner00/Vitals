using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vitals.Integrations.Migrations
{
    /// <inheritdoc />
    public partial class FixOneToOneRelationShip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Votes_VoteId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_VoteId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "VoteId",
                table: "Posts");

            migrationBuilder.AddColumn<int>(
                name: "PostId",
                table: "Votes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_PostId",
                table: "Votes",
                column: "PostId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Posts_PostId",
                table: "Votes",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Posts_PostId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_PostId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Votes");

            migrationBuilder.AddColumn<int>(
                name: "VoteId",
                table: "Posts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_VoteId",
                table: "Posts",
                column: "VoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Votes_VoteId",
                table: "Posts",
                column: "VoteId",
                principalTable: "Votes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
