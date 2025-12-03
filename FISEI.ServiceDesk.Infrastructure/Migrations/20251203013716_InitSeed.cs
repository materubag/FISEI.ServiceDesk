using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FISEI.ServiceDesk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosIncidencia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EsFinal = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosIncidencia", x => x.Id);
                });

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
                name: "Prioridades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Peso = table.Column<int>(type: "int", nullable: false),
                    TiempoMaxHoras = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prioridades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Servicios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Servicios_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioDestinoId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Leida = table.Column<bool>(type: "bit", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Usuarios_UsuarioDestinoId",
                        column: x => x.UsuarioDestinoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Incidencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoId = table.Column<int>(type: "int", nullable: false),
                    PrioridadId = table.Column<int>(type: "int", nullable: false),
                    ServicioId = table.Column<int>(type: "int", nullable: false),
                    CreadorId = table.Column<int>(type: "int", nullable: false),
                    TecnicoAsignadoId = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    FechaUltimoCambio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Cerrada = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    LaboratorioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incidencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incidencias_EstadosIncidencia_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "EstadosIncidencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incidencias_Laboratorios_LaboratorioId",
                        column: x => x.LaboratorioId,
                        principalTable: "Laboratorios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Incidencias_Prioridades_PrioridadId",
                        column: x => x.PrioridadId,
                        principalTable: "Prioridades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incidencias_Servicios_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "Servicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incidencias_Usuarios_CreadorId",
                        column: x => x.CreadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incidencias_Usuarios_TecnicoAsignadoId",
                        column: x => x.TecnicoAsignadoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SLA_Definiciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrioridadId = table.Column<int>(type: "int", nullable: false),
                    ServicioId = table.Column<int>(type: "int", nullable: true),
                    HorasRespuesta = table.Column<int>(type: "int", nullable: false),
                    HorasResolucion = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SLA_Definiciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SLA_Definiciones_Prioridades_PrioridadId",
                        column: x => x.PrioridadId,
                        principalTable: "Prioridades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SLA_Definiciones_Servicios_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "Servicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateTable(
                name: "ComentariosIncidencia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidenciaId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EsInterno = table.Column<bool>(type: "bit", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComentariosIncidencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComentariosIncidencia_Incidencias_IncidenciaId",
                        column: x => x.IncidenciaId,
                        principalTable: "Incidencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComentariosIncidencia_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Escalaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidenciaId = table.Column<int>(type: "int", nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Escalaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Escalaciones_Incidencias_IncidenciaId",
                        column: x => x.IncidenciaId,
                        principalTable: "Incidencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidenciaId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Puntuacion = table.Column<byte>(type: "tinyint", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Incidencias_IncidenciaId",
                        column: x => x.IncidenciaId,
                        principalTable: "Incidencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Seguimientos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidenciaId = table.Column<int>(type: "int", nullable: false),
                    EstadoAnteriorId = table.Column<int>(type: "int", nullable: true),
                    EstadoNuevoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seguimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seguimientos_EstadosIncidencia_EstadoAnteriorId",
                        column: x => x.EstadoAnteriorId,
                        principalTable: "EstadosIncidencia",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Seguimientos_EstadosIncidencia_EstadoNuevoId",
                        column: x => x.EstadoNuevoId,
                        principalTable: "EstadosIncidencia",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Seguimientos_Incidencias_IncidenciaId",
                        column: x => x.IncidenciaId,
                        principalTable: "Incidencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Seguimientos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SLA_Incidencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidenciaId = table.Column<int>(type: "int", nullable: false),
                    FechaLimiteRespuesta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaLimiteResolucion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CumplidoRespuesta = table.Column<bool>(type: "bit", nullable: false),
                    CumplidoResolucion = table.Column<bool>(type: "bit", nullable: false),
                    CreadoUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SLA_Incidencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SLA_Incidencias_Incidencias_IncidenciaId",
                        column: x => x.IncidenciaId,
                        principalTable: "Incidencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Activo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Hardware" },
                    { 2, true, "Software" },
                    { 3, true, "Red" },
                    { 4, true, "Acceso" },
                    { 5, true, "Audiovisual" },
                    { 6, true, "Climatización" },
                    { 7, true, "Mobiliario" },
                    { 8, true, "Otro" }
                });

            migrationBuilder.InsertData(
                table: "EstadosIncidencia",
                columns: new[] { "Id", "Codigo", "EsFinal", "Orden" },
                values: new object[,]
                {
                    { 1, "ABIERTO", false, 1 },
                    { 2, "EN_PROGRESO", false, 2 },
                    { 3, "RESUELTO", false, 3 },
                    { 4, "CERRADO", true, 4 },
                    { 5, "REABIERTO", false, 5 }
                });

            migrationBuilder.InsertData(
                table: "Laboratorios",
                columns: new[] { "Id", "Activo", "Codigo", "Edificio", "Nombre", "Ubicacion" },
                values: new object[,]
                {
                    { 1, true, "LAB-RED", "FISEI", "Lab Redes", "Piso 2" },
                    { 2, true, "LAB-SW", "FISEI", "Lab Software", "Piso 3" },
                    { 3, true, "LAB-HW", "FISEI", "Lab Hardware", "Piso 1" },
                    { 4, true, "LAB-AV", "FISEI", "Lab Audiovisual", "Piso 4" }
                });

            migrationBuilder.InsertData(
                table: "Prioridades",
                columns: new[] { "Id", "Nombre", "Peso", "TiempoMaxHoras" },
                values: new object[,]
                {
                    { 1, "Baja", 1, 168 },
                    { 2, "Media", 2, 72 },
                    { 3, "Alta", 3, 24 },
                    { 4, "Crítica", 4, 4 }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Estudiante" },
                    { 2, "Tecnico" },
                    { 3, "Administrador" },
                    { 4, "Docente" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "Correo", "FechaRegistro", "Nombre", "PasswordHash", "PasswordSalt", "RolId" },
                values: new object[,]
                {
                    { 1, true, "admin1@admin.com", new DateTime(2025, 12, 3, 1, 37, 15, 965, DateTimeKind.Utc).AddTicks(8376), "Admin 1", "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=", "lOvvglGCDYWu2T3sCDzE1A==", 3 },
                    { 2, true, "admin2@admin.com", new DateTime(2025, 12, 3, 1, 37, 15, 965, DateTimeKind.Utc).AddTicks(8376), "Admin 2", "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=", "lOvvglGCDYWu2T3sCDzE1A==", 3 },
                    { 3, true, "tecnico1@uta.edu.ec", new DateTime(2025, 12, 3, 1, 37, 15, 965, DateTimeKind.Utc).AddTicks(8376), "Tecnico 1", "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=", "lOvvglGCDYWu2T3sCDzE1A==", 2 },
                    { 4, true, "tecnico2@uta.edu.ec", new DateTime(2025, 12, 3, 1, 37, 15, 965, DateTimeKind.Utc).AddTicks(8376), "Tecnico 2", "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=", "lOvvglGCDYWu2T3sCDzE1A==", 2 },
                    { 5, true, "tecnico3@uta.edu.ec", new DateTime(2025, 12, 3, 1, 37, 15, 965, DateTimeKind.Utc).AddTicks(8376), "Tecnico 3", "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=", "lOvvglGCDYWu2T3sCDzE1A==", 2 },
                    { 6, true, "tecnico4@uta.edu.ec", new DateTime(2025, 12, 3, 1, 37, 15, 965, DateTimeKind.Utc).AddTicks(8376), "Tecnico 4", "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=", "lOvvglGCDYWu2T3sCDzE1A==", 2 },
                    { 7, true, "docente1@uta.edu.ec", new DateTime(2025, 12, 3, 1, 37, 15, 965, DateTimeKind.Utc).AddTicks(8376), "Docente 1", "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=", "lOvvglGCDYWu2T3sCDzE1A==", 4 },
                    { 8, true, "docente2@uta.edu.ec", new DateTime(2025, 12, 3, 1, 37, 15, 965, DateTimeKind.Utc).AddTicks(8376), "Docente 2", "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=", "lOvvglGCDYWu2T3sCDzE1A==", 4 },
                    { 9, true, "docente3@uta.edu.ec", new DateTime(2025, 12, 3, 1, 37, 15, 965, DateTimeKind.Utc).AddTicks(8376), "Docente 3", "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=", "lOvvglGCDYWu2T3sCDzE1A==", 4 },
                    { 10, true, "docente4@uta.edu.ec", new DateTime(2025, 12, 3, 1, 37, 15, 965, DateTimeKind.Utc).AddTicks(8376), "Docente 4", "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=", "lOvvglGCDYWu2T3sCDzE1A==", 4 },
                    { 11, true, "est1@uta.edu.ec", new DateTime(2025, 12, 3, 1, 37, 15, 965, DateTimeKind.Utc).AddTicks(8376), "Estudiante 1", "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=", "lOvvglGCDYWu2T3sCDzE1A==", 1 },
                    { 12, true, "est2@uta.edu.ec", new DateTime(2025, 12, 3, 1, 37, 15, 965, DateTimeKind.Utc).AddTicks(8376), "Estudiante 2", "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=", "lOvvglGCDYWu2T3sCDzE1A==", 1 },
                    { 13, true, "est3@uta.edu.ec", new DateTime(2025, 12, 3, 1, 37, 15, 965, DateTimeKind.Utc).AddTicks(8376), "Estudiante 3", "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=", "lOvvglGCDYWu2T3sCDzE1A==", 1 },
                    { 14, true, "est4@uta.edu.ec", new DateTime(2025, 12, 3, 1, 37, 15, 965, DateTimeKind.Utc).AddTicks(8376), "Estudiante 4", "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=", "lOvvglGCDYWu2T3sCDzE1A==", 1 },
                    { 15, true, "est5@uta.edu.ec", new DateTime(2025, 12, 3, 1, 37, 15, 965, DateTimeKind.Utc).AddTicks(8376), "Estudiante 5", "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=", "lOvvglGCDYWu2T3sCDzE1A==", 1 },
                    { 16, true, "est6@uta.edu.ec", new DateTime(2025, 12, 3, 1, 37, 15, 965, DateTimeKind.Utc).AddTicks(8376), "Estudiante 6", "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=", "lOvvglGCDYWu2T3sCDzE1A==", 1 },
                    { 17, true, "est7@uta.edu.ec", new DateTime(2025, 12, 3, 1, 37, 15, 965, DateTimeKind.Utc).AddTicks(8376), "Estudiante 7", "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=", "lOvvglGCDYWu2T3sCDzE1A==", 1 },
                    { 18, true, "est8@uta.edu.ec", new DateTime(2025, 12, 3, 1, 37, 15, 965, DateTimeKind.Utc).AddTicks(8376), "Estudiante 8", "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=", "lOvvglGCDYWu2T3sCDzE1A==", 1 },
                    { 19, true, "est9@uta.edu.ec", new DateTime(2025, 12, 3, 1, 37, 15, 965, DateTimeKind.Utc).AddTicks(8376), "Estudiante 9", "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=", "lOvvglGCDYWu2T3sCDzE1A==", 1 },
                    { 20, true, "est10@uta.edu.ec", new DateTime(2025, 12, 3, 1, 37, 15, 965, DateTimeKind.Utc).AddTicks(8376), "Estudiante 10", "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=", "lOvvglGCDYWu2T3sCDzE1A==", 1 }
                });

            migrationBuilder.InsertData(
                table: "SLA_Definiciones",
                columns: new[] { "Id", "Activo", "HorasResolucion", "HorasRespuesta", "PrioridadId", "ServicioId" },
                values: new object[,]
                {
                    { 1, true, 4, 0, 4, null },
                    { 2, true, 24, 1, 3, null },
                    { 3, true, 72, 4, 2, null },
                    { 4, true, 168, 8, 1, null }
                });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "Id", "Activo", "CategoriaId", "Nombre" },
                values: new object[,]
                {
                    { 1, true, 1, "Soporte PC" },
                    { 2, true, 3, "Red/Conectividad" },
                    { 3, true, 2, "Software" },
                    { 4, true, 5, "Audiovisual" }
                });

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
                name: "IX_ComentariosIncidencia_IncidenciaId",
                table: "ComentariosIncidencia",
                column: "IncidenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_ComentariosIncidencia_UsuarioId",
                table: "ComentariosIncidencia",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Escalaciones_IncidenciaId",
                table: "Escalaciones",
                column: "IncidenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_EstadosIncidencia_Codigo",
                table: "EstadosIncidencia",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_IncidenciaId",
                table: "Feedbacks",
                column: "IncidenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UsuarioId",
                table: "Feedbacks",
                column: "UsuarioId");

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
                name: "IX_Laboratorios_Codigo",
                table: "Laboratorios",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioDestinoId",
                table: "Notificaciones",
                column: "UsuarioDestinoId");

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
                name: "IX_Servicios_CategoriaId",
                table: "Servicios",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_SLA_Definiciones_PrioridadId",
                table: "SLA_Definiciones",
                column: "PrioridadId");

            migrationBuilder.CreateIndex(
                name: "IX_SLA_Definiciones_ServicioId",
                table: "SLA_Definiciones",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_SLA_Incidencias_IncidenciaId",
                table: "SLA_Incidencias",
                column: "IncidenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticulosConocimiento");

            migrationBuilder.DropTable(
                name: "ComentariosIncidencia");

            migrationBuilder.DropTable(
                name: "Escalaciones");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Seguimientos");

            migrationBuilder.DropTable(
                name: "SLA_Definiciones");

            migrationBuilder.DropTable(
                name: "SLA_Incidencias");

            migrationBuilder.DropTable(
                name: "Incidencias");

            migrationBuilder.DropTable(
                name: "EstadosIncidencia");

            migrationBuilder.DropTable(
                name: "Laboratorios");

            migrationBuilder.DropTable(
                name: "Prioridades");

            migrationBuilder.DropTable(
                name: "Servicios");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}
