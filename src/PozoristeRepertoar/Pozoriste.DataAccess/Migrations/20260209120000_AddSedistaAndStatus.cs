using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pozoriste.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSedistaAndStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BrojRedova",
                table: "Sala",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "SedistaPoRedu",
                table: "Sala",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Rezervacija",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RezervacijaSediste",
                columns: table => new
                {
                    RezervacijaSedisteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RezervacijaId = table.Column<int>(type: "int", nullable: false),
                    TerminId = table.Column<int>(type: "int", nullable: false),
                    Red = table.Column<int>(type: "int", nullable: false),
                    Broj = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RezervacijaSediste", x => x.RezervacijaSedisteId);
                    table.ForeignKey(
                        name: "FK_RezervacijaSediste_Rezervacija_RezervacijaId",
                        column: x => x.RezervacijaId,
                        principalTable: "Rezervacija",
                        principalColumn: "RezervacijaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RezervacijaSediste_Termin_TerminId",
                        column: x => x.TerminId,
                        principalTable: "Termin",
                        principalColumn: "TerminId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RezervacijaSediste_RezervacijaId",
                table: "RezervacijaSediste",
                column: "RezervacijaId");

            migrationBuilder.CreateIndex(
                name: "IX_RezervacijaSediste_TerminId",
                table: "RezervacijaSediste",
                column: "TerminId");

            migrationBuilder.CreateIndex(
                name: "IX_RezervacijaSediste_TerminId_Red_Broj",
                table: "RezervacijaSediste",
                columns: new[] { "TerminId", "Red", "Broj" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RezervacijaSediste");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Rezervacija");

            migrationBuilder.DropColumn(
                name: "BrojRedova",
                table: "Sala");

            migrationBuilder.DropColumn(
                name: "SedistaPoRedu",
                table: "Sala");
        }
    }
}
