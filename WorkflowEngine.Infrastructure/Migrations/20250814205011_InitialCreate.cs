using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkflowEngine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Campos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MensajeCliente = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RegexCliente = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RegexServidor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EsInterno = table.Column<bool>(type: "bit", nullable: false),
                    TipoDato = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposFlujo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IdSecuenciaInicial = table.Column<int>(type: "int", nullable: false),
                    IdSecuenciaFinal = table.Column<int>(type: "int", nullable: false),
                    OrdenSecuencias = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposFlujo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlujosActivos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTipoFlujo = table.Column<int>(type: "int", nullable: false),
                    IdSecuenciaActual = table.Column<int>(type: "int", nullable: false),
                    EstadoFlujo = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FechaCompletado = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlujosActivos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlujosActivos_TiposFlujo_IdTipoFlujo",
                        column: x => x.IdTipoFlujo,
                        principalTable: "TiposFlujo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Secuencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTipoFlujo = table.Column<int>(type: "int", nullable: false),
                    ListaIdPasos = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secuencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Secuencias_TiposFlujo_IdTipoFlujo",
                        column: x => x.IdTipoFlujo,
                        principalTable: "TiposFlujo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CamposFlujosActivos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdFlujoActivo = table.Column<int>(type: "int", nullable: false),
                    IdCampo = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CamposFlujosActivos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CamposFlujosActivos_Campos_IdCampo",
                        column: x => x.IdCampo,
                        principalTable: "Campos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CamposFlujosActivos_FlujosActivos_IdFlujoActivo",
                        column: x => x.IdFlujoActivo,
                        principalTable: "FlujosActivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pasos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoPaso = table.Column<int>(type: "int", nullable: false),
                    ListaIdCampos = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SecuenciaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pasos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pasos_Secuencias_SecuenciaId",
                        column: x => x.SecuenciaId,
                        principalTable: "Secuencias",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PasosCampos",
                columns: table => new
                {
                    IdPaso = table.Column<int>(type: "int", nullable: false),
                    IdCampo = table.Column<int>(type: "int", nullable: false),
                    EsRequerido = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasosCampos", x => new { x.IdPaso, x.IdCampo });
                    table.ForeignKey(
                        name: "FK_PasosCampos_Campos_IdCampo",
                        column: x => x.IdCampo,
                        principalTable: "Campos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PasosCampos_Pasos_IdPaso",
                        column: x => x.IdPaso,
                        principalTable: "Pasos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CamposFlujosActivos_IdCampo",
                table: "CamposFlujosActivos",
                column: "IdCampo");

            migrationBuilder.CreateIndex(
                name: "IX_CamposFlujosActivos_IdFlujoActivo_IdCampo",
                table: "CamposFlujosActivos",
                columns: new[] { "IdFlujoActivo", "IdCampo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlujosActivos_IdTipoFlujo",
                table: "FlujosActivos",
                column: "IdTipoFlujo");

            migrationBuilder.CreateIndex(
                name: "IX_Pasos_SecuenciaId",
                table: "Pasos",
                column: "SecuenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_PasosCampos_IdCampo",
                table: "PasosCampos",
                column: "IdCampo");

            migrationBuilder.CreateIndex(
                name: "IX_Secuencias_IdTipoFlujo",
                table: "Secuencias",
                column: "IdTipoFlujo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CamposFlujosActivos");

            migrationBuilder.DropTable(
                name: "PasosCampos");

            migrationBuilder.DropTable(
                name: "FlujosActivos");

            migrationBuilder.DropTable(
                name: "Campos");

            migrationBuilder.DropTable(
                name: "Pasos");

            migrationBuilder.DropTable(
                name: "Secuencias");

            migrationBuilder.DropTable(
                name: "TiposFlujo");
        }
    }
}
