using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pozoriste.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddZonePricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rezervacija_Termin_TerminId",
                table: "Rezervacija");

            migrationBuilder.DropForeignKey(
                name: "FK_RezervacijaSediste_Termin_TerminId",
                table: "RezervacijaSediste");

            migrationBuilder.DropIndex(
                name: "IX_RezervacijaSediste_TerminId",
                table: "RezervacijaSediste");

            migrationBuilder.AddColumn<decimal>(
                name: "Cena",
                table: "RezervacijaSediste",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Zona",
                table: "RezervacijaSediste",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "UkupnaCena",
                table: "Rezervacija",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_Rezervacija_Termin_TerminId",
                table: "Rezervacija",
                column: "TerminId",
                principalTable: "Termin",
                principalColumn: "TerminId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RezervacijaSediste_Termin_TerminId",
                table: "RezervacijaSediste",
                column: "TerminId",
                principalTable: "Termin",
                principalColumn: "TerminId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rezervacija_Termin_TerminId",
                table: "Rezervacija");

            migrationBuilder.DropForeignKey(
                name: "FK_RezervacijaSediste_Termin_TerminId",
                table: "RezervacijaSediste");

            migrationBuilder.DropColumn(
                name: "Cena",
                table: "RezervacijaSediste");

            migrationBuilder.DropColumn(
                name: "Zona",
                table: "RezervacijaSediste");

            migrationBuilder.DropColumn(
                name: "UkupnaCena",
                table: "Rezervacija");

            migrationBuilder.CreateIndex(
                name: "IX_RezervacijaSediste_TerminId",
                table: "RezervacijaSediste",
                column: "TerminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rezervacija_Termin_TerminId",
                table: "Rezervacija",
                column: "TerminId",
                principalTable: "Termin",
                principalColumn: "TerminId");

            migrationBuilder.AddForeignKey(
                name: "FK_RezervacijaSediste_Termin_TerminId",
                table: "RezervacijaSediste",
                column: "TerminId",
                principalTable: "Termin",
                principalColumn: "TerminId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
