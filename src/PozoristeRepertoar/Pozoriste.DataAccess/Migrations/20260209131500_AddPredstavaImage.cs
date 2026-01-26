using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pozoriste.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPredstavaImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SlikaUrl",
                table: "Predstava",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SlikaUrl",
                table: "Predstava");
        }
    }
}
