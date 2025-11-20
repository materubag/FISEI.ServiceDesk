using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FISEI.ServiceDesk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationshipsAndKnowledgeBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Prioridades",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Prioridades",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Prioridades",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Prioridades",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SLA_Definiciones",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SLA_Definiciones",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SLA_Definiciones",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SLA_Definiciones",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "LaboratorioId",
                table: "Incidencias",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Laboratorios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Edificio = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Ubicacion = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Laboratorios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticulosConocimiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Referencias = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Etiquetas = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ServicioId = table.Column<int>(type: "int", nullable: true),
                    LaboratorioId = table.Column<int>(type: "int", nullable: true),
                    AutorId = table.Column<int>(type: "int", nullable: false),
                    IncidenciaOrigenId = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticulosConocimiento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticulosConocimiento_Incidencias_IncidenciaOrigenId",
                        column: x => x.IncidenciaOrigenId,
                        principalTable: "Incidencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ArticulosConocimiento_Laboratorios_LaboratorioId",
                        column: x => x.LaboratorioId,
                        principalTable: "Laboratorios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ArticulosConocimiento_Servicios_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "Servicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArticulosConocimiento_Usuarios_AutorId",
                        column: x => x.AutorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SLA_Incidencias_IncidenciaId",
                table: "SLA_Incidencias",
                column: "IncidenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_SLA_Definiciones_PrioridadId",
                table: "SLA_Definiciones",
                column: "PrioridadId");

            migrationBuilder.CreateIndex(
                name: "IX_SLA_Definiciones_ServicioId",
                table: "SLA_Definiciones",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_CategoriaId",
                table: "Servicios",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Seguimientos_EstadoAnteriorId",
                table: "Seguimientos",
                column: "EstadoAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_Seguimientos_EstadoNuevoId",
                table: "Seguimientos",
                column: "EstadoNuevoId");

            migrationBuilder.CreateIndex(
                name: "IX_Seguimientos_IncidenciaId",
                table: "Seguimientos",
                column: "IncidenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Seguimientos_UsuarioId",
                table: "Seguimientos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioDestinoId",
                table: "Notificaciones",
                column: "UsuarioDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidencias_CreadorId",
                table: "Incidencias",
                column: "CreadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidencias_EstadoId",
                table: "Incidencias",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidencias_LaboratorioId",
                table: "Incidencias",
                column: "LaboratorioId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidencias_PrioridadId",
                table: "Incidencias",
                column: "PrioridadId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidencias_ServicioId",
                table: "Incidencias",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidencias_TecnicoAsignadoId",
                table: "Incidencias",
                column: "TecnicoAsignadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_IncidenciaId",
                table: "Feedbacks",
                column: "IncidenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UsuarioId",
                table: "Feedbacks",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Escalaciones_IncidenciaId",
                table: "Escalaciones",
                column: "IncidenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_ComentariosIncidencia_IncidenciaId",
                table: "ComentariosIncidencia",
                column: "IncidenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_ComentariosIncidencia_UsuarioId",
                table: "ComentariosIncidencia",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticulosConocimiento_AutorId_Activo",
                table: "ArticulosConocimiento",
                columns: new[] { "AutorId", "Activo" });

            migrationBuilder.CreateIndex(
                name: "IX_ArticulosConocimiento_LaboratorioId",
                table: "ArticulosConocimiento",
                column: "LaboratorioId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticulosConocimiento_ServicioId_LaboratorioId",
                table: "ArticulosConocimiento",
                columns: new[] { "ServicioId", "LaboratorioId" });

            migrationBuilder.CreateIndex(
                name: "IX_KB_IncidenciaOrigen",
                table: "ArticulosConocimiento",
                column: "IncidenciaOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_Laboratorios_Codigo",
                table: "Laboratorios",
                column: "Codigo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ComentariosIncidencia_Incidencias_IncidenciaId",
                table: "ComentariosIncidencia",
                column: "IncidenciaId",
                principalTable: "Incidencias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ComentariosIncidencia_Usuarios_UsuarioId",
                table: "ComentariosIncidencia",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Escalaciones_Incidencias_IncidenciaId",
                table: "Escalaciones",
                column: "IncidenciaId",
                principalTable: "Incidencias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Incidencias_IncidenciaId",
                table: "Feedbacks",
                column: "IncidenciaId",
                principalTable: "Incidencias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Usuarios_UsuarioId",
                table: "Feedbacks",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Incidencias_EstadosIncidencia_EstadoId",
                table: "Incidencias",
                column: "EstadoId",
                principalTable: "EstadosIncidencia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Incidencias_Laboratorios_LaboratorioId",
                table: "Incidencias",
                column: "LaboratorioId",
                principalTable: "Laboratorios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Incidencias_Prioridades_PrioridadId",
                table: "Incidencias",
                column: "PrioridadId",
                principalTable: "Prioridades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Incidencias_Servicios_ServicioId",
                table: "Incidencias",
                column: "ServicioId",
                principalTable: "Servicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Incidencias_Usuarios_CreadorId",
                table: "Incidencias",
                column: "CreadorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Incidencias_Usuarios_TecnicoAsignadoId",
                table: "Incidencias",
                column: "TecnicoAsignadoId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_Usuarios_UsuarioDestinoId",
                table: "Notificaciones",
                column: "UsuarioDestinoId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Seguimientos_EstadosIncidencia_EstadoAnteriorId",
                table: "Seguimientos",
                column: "EstadoAnteriorId",
                principalTable: "EstadosIncidencia",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Seguimientos_EstadosIncidencia_EstadoNuevoId",
                table: "Seguimientos",
                column: "EstadoNuevoId",
                principalTable: "EstadosIncidencia",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Seguimientos_Incidencias_IncidenciaId",
                table: "Seguimientos",
                column: "IncidenciaId",
                principalTable: "Incidencias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Seguimientos_Usuarios_UsuarioId",
                table: "Seguimientos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_Categorias_CategoriaId",
                table: "Servicios",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SLA_Definiciones_Prioridades_PrioridadId",
                table: "SLA_Definiciones",
                column: "PrioridadId",
                principalTable: "Prioridades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SLA_Definiciones_Servicios_ServicioId",
                table: "SLA_Definiciones",
                column: "ServicioId",
                principalTable: "Servicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SLA_Incidencias_Incidencias_IncidenciaId",
                table: "SLA_Incidencias",
                column: "IncidenciaId",
                principalTable: "Incidencias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComentariosIncidencia_Incidencias_IncidenciaId",
                table: "ComentariosIncidencia");

            migrationBuilder.DropForeignKey(
                name: "FK_ComentariosIncidencia_Usuarios_UsuarioId",
                table: "ComentariosIncidencia");

            migrationBuilder.DropForeignKey(
                name: "FK_Escalaciones_Incidencias_IncidenciaId",
                table: "Escalaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Incidencias_IncidenciaId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Usuarios_UsuarioId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidencias_EstadosIncidencia_EstadoId",
                table: "Incidencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidencias_Laboratorios_LaboratorioId",
                table: "Incidencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidencias_Prioridades_PrioridadId",
                table: "Incidencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidencias_Servicios_ServicioId",
                table: "Incidencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidencias_Usuarios_CreadorId",
                table: "Incidencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidencias_Usuarios_TecnicoAsignadoId",
                table: "Incidencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_Usuarios_UsuarioDestinoId",
                table: "Notificaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Seguimientos_EstadosIncidencia_EstadoAnteriorId",
                table: "Seguimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_Seguimientos_EstadosIncidencia_EstadoNuevoId",
                table: "Seguimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_Seguimientos_Incidencias_IncidenciaId",
                table: "Seguimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_Seguimientos_Usuarios_UsuarioId",
                table: "Seguimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_Categorias_CategoriaId",
                table: "Servicios");

            migrationBuilder.DropForeignKey(
                name: "FK_SLA_Definiciones_Prioridades_PrioridadId",
                table: "SLA_Definiciones");

            migrationBuilder.DropForeignKey(
                name: "FK_SLA_Definiciones_Servicios_ServicioId",
                table: "SLA_Definiciones");

            migrationBuilder.DropForeignKey(
                name: "FK_SLA_Incidencias_Incidencias_IncidenciaId",
                table: "SLA_Incidencias");

            migrationBuilder.DropTable(
                name: "ArticulosConocimiento");

            migrationBuilder.DropTable(
                name: "Laboratorios");

            migrationBuilder.DropIndex(
                name: "IX_SLA_Incidencias_IncidenciaId",
                table: "SLA_Incidencias");

            migrationBuilder.DropIndex(
                name: "IX_SLA_Definiciones_PrioridadId",
                table: "SLA_Definiciones");

            migrationBuilder.DropIndex(
                name: "IX_SLA_Definiciones_ServicioId",
                table: "SLA_Definiciones");

            migrationBuilder.DropIndex(
                name: "IX_Servicios_CategoriaId",
                table: "Servicios");

            migrationBuilder.DropIndex(
                name: "IX_Seguimientos_EstadoAnteriorId",
                table: "Seguimientos");

            migrationBuilder.DropIndex(
                name: "IX_Seguimientos_EstadoNuevoId",
                table: "Seguimientos");

            migrationBuilder.DropIndex(
                name: "IX_Seguimientos_IncidenciaId",
                table: "Seguimientos");

            migrationBuilder.DropIndex(
                name: "IX_Seguimientos_UsuarioId",
                table: "Seguimientos");

            migrationBuilder.DropIndex(
                name: "IX_Notificaciones_UsuarioDestinoId",
                table: "Notificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Incidencias_CreadorId",
                table: "Incidencias");

            migrationBuilder.DropIndex(
                name: "IX_Incidencias_EstadoId",
                table: "Incidencias");

            migrationBuilder.DropIndex(
                name: "IX_Incidencias_LaboratorioId",
                table: "Incidencias");

            migrationBuilder.DropIndex(
                name: "IX_Incidencias_PrioridadId",
                table: "Incidencias");

            migrationBuilder.DropIndex(
                name: "IX_Incidencias_ServicioId",
                table: "Incidencias");

            migrationBuilder.DropIndex(
                name: "IX_Incidencias_TecnicoAsignadoId",
                table: "Incidencias");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_IncidenciaId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_UsuarioId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Escalaciones_IncidenciaId",
                table: "Escalaciones");

            migrationBuilder.DropIndex(
                name: "IX_ComentariosIncidencia_IncidenciaId",
                table: "ComentariosIncidencia");

            migrationBuilder.DropIndex(
                name: "IX_ComentariosIncidencia_UsuarioId",
                table: "ComentariosIncidencia");

            migrationBuilder.DropColumn(
                name: "LaboratorioId",
                table: "Incidencias");

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Activo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Redes" },
                    { 2, true, "Ofimática" },
                    { 3, true, "AulaVirtual" }
                });

            migrationBuilder.InsertData(
                table: "Prioridades",
                columns: new[] { "Id", "Nombre", "Peso", "TiempoMaxHoras" },
                values: new object[,]
                {
                    { 1, "Baja", 1, 72 },
                    { 2, "Media", 2, 48 },
                    { 3, "Alta", 3, 24 },
                    { 4, "Critica", 4, 8 }
                });

            migrationBuilder.InsertData(
                table: "SLA_Definiciones",
                columns: new[] { "Id", "Activo", "HorasResolucion", "HorasRespuesta", "PrioridadId", "ServicioId" },
                values: new object[,]
                {
                    { 1, true, 72, 24, 1, null },
                    { 2, true, 48, 12, 2, null },
                    { 3, true, 24, 4, 3, null },
                    { 4, true, 8, 2, 4, null }
                });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "Id", "Activo", "CategoriaId", "Nombre" },
                values: new object[,]
                {
                    { 1, true, 1, "WiFi" },
                    { 2, true, 1, "Ethernet" },
                    { 3, true, 2, "Correo" },
                    { 4, true, 2, "Impresoras" },
                    { 5, true, 3, "AulaVirtual" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "Correo", "FechaRegistro", "Nombre", "PasswordHash", "PasswordSalt", "RolId" },
                values: new object[,]
                {
                    { 1, true, "estudiante@demo.local", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Estudiante Demo", "REEMPLAZA_HASH", "REEMPLAZA_SALT", 1 },
                    { 2, true, "tecnico@demo.local", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tecnico Demo", "REEMPLAZA_HASH", "REEMPLAZA_SALT", 2 },
                    { 3, true, "admin@demo.local", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin Demo", "REEMPLAZA_HASH", "REEMPLAZA_SALT", 3 }
                });
        }
    }
}
