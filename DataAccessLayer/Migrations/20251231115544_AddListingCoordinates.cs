using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddListingCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Enlem",
                table: "Ilanlar",
                type: "decimal(9,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Boylam",
                table: "Ilanlar",
                type: "decimal(9,6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ilanlar_Enlem_Boylam",
                table: "Ilanlar",
                columns: new[] { "Enlem", "Boylam" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ilanlar_Enlem_Boylam",
                table: "Ilanlar");

            migrationBuilder.DropColumn(
                name: "Enlem",
                table: "Ilanlar");

            migrationBuilder.DropColumn(
                name: "Boylam",
                table: "Ilanlar");
        }
    }
}
