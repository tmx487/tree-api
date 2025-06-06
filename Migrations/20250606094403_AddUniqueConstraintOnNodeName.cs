using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TreeAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintOnNodeName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Nodes_TreeId",
                table: "Nodes");

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_TreeId_Name",
                table: "Nodes",
                columns: new[] { "TreeId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Nodes_TreeId_Name",
                table: "Nodes");

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_TreeId",
                table: "Nodes",
                column: "TreeId");
        }
    }
}
