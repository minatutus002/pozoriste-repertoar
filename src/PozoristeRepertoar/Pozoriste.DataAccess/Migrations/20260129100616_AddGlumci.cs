using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pozoriste.DataAccess.Migrations
{
    public partial class AddGlumci : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.CreateTable(
                name: "Glumci",
                columns: table => new
                {
                    GlumacId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PunoIme = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Glumci", x => x.GlumacId);
                });

            migrationBuilder.CreateTable(
                name: "PredstavaGlumci",
                columns: table => new
                {
                    PredstavaId = table.Column<int>(type: "int", nullable: false),
                    GlumacId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredstavaGlumci", x => new { x.PredstavaId, x.GlumacId });

                    table.ForeignKey(
                        name: "FK_PredstavaGlumci_Glumci_GlumacId",
                        column: x => x.GlumacId,
                        principalTable: "Glumci",
                        principalColumn: "GlumacId",
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: "FK_PredstavaGlumci_Predstava_PredstavaId",
                        column: x => x.PredstavaId,
                        principalTable: "Predstava",
                        principalColumn: "PredstavaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PredstavaGlumci_GlumacId",
                table: "PredstavaGlumci",
                column: "GlumacId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PredstavaGlumci");

            migrationBuilder.DropTable(
                name: "Glumci");
        }
    }
}
