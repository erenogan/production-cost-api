using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UretimMaliyet.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hammaddeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", nullable: false),
                    Birim = table.Column<int>(type: "INTEGER", nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "TEXT", nullable: false),
                    StokMiktari = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hammaddeler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Urunler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", nullable: false),
                    SatisFiyati = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urunler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReceteKalemleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UrunId = table.Column<int>(type: "INTEGER", nullable: false),
                    HammaddeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Miktar = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceteKalemleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceteKalemleri_Hammaddeler_HammaddeId",
                        column: x => x.HammaddeId,
                        principalTable: "Hammaddeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceteKalemleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceteKalemleri_HammaddeId",
                table: "ReceteKalemleri",
                column: "HammaddeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceteKalemleri_UrunId",
                table: "ReceteKalemleri",
                column: "UrunId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceteKalemleri");

            migrationBuilder.DropTable(
                name: "Hammaddeler");

            migrationBuilder.DropTable(
                name: "Urunler");
        }
    }
}
