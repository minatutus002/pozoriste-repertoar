using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pozoriste.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitPozoriste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rezervacije");

            migrationBuilder.CreateTable(
                name: "Rezervacija",
                columns: table => new
                {
                    RezervacijaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TerminId = table.Column<int>(type: "int", nullable: false),
                    BrojKarata = table.Column<int>(type: "int", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervacija", x => x.RezervacijaId);
                    table.ForeignKey(
                        name: "FK_Rezervacija_Termin_TerminId",
                        column: x => x.TerminId,
                        principalTable: "Termin",
                        principalColumn: "TerminId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacija_TerminId",
                table: "Rezervacija",
                column: "TerminId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rezervacija");

            migrationBuilder.CreateTable(
                name: "Rezervacije",
                columns: table => new
                {
                    RezervacijaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TerminId = table.Column<int>(type: "int", nullable: false),
                    BrojKarata = table.Column<int>(type: "int", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervacije", x => x.RezervacijaId);
                    table.ForeignKey(
                        name: "FK_Rezervacije_Termin_TerminId",
                        column: x => x.TerminId,
                        principalTable: "Termin",
                        principalColumn: "TerminId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_TerminId",
                table: "Rezervacije",
                column: "TerminId");
        }
    }
}
