using Microsoft.EntityFrameworkCore;
using FISEI.ServiceDesk.Domain.Entities;

namespace FISEI.ServiceDesk.Infrastructure.Persistence;

public class ServiceDeskDbContext : DbContext
{
    public ServiceDeskDbContext(DbContextOptions<ServiceDeskDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Incidencia> Incidencias => Set<Incidencia>();
    public DbSet<Seguimiento> Seguimientos => Set<Seguimiento>();
    public DbSet<ComentarioIncidencia> ComentariosIncidencia => Set<ComentarioIncidencia>();
    public DbSet<EstadoIncidencia> EstadosIncidencia => Set<EstadoIncidencia>();
    public DbSet<Prioridad> Prioridades => Set<Prioridad>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Servicio> Servicios => Set<Servicio>();
    public DbSet<FeedbackIncidencia> Feedbacks => Set<FeedbackIncidencia>();
    public DbSet<Notificacion> Notificaciones => Set<Notificacion>();
    public DbSet<SLA_Definicion> SLA_Definiciones => Set<SLA_Definicion>();
    public DbSet<SLA_Incidencia> SLA_Incidencias => Set<SLA_Incidencia>();
    public DbSet<Escalacion> Escalaciones => Set<Escalacion>();
    public DbSet<Laboratorio> Laboratorios => Set<Laboratorio>();
    public DbSet<ArticuloConocimiento> ArticulosConocimiento => Set<ArticuloConocimiento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Constraints e índices base
        modelBuilder.Entity<Usuario>().HasIndex(u => u.Correo).IsUnique();
        modelBuilder.Entity<Incidencia>().Property(i => i.Titulo).HasMaxLength(200);
        modelBuilder.Entity<EstadoIncidencia>().HasIndex(e => e.Codigo).IsUnique();

        modelBuilder.Entity<Laboratorio>(b =>
        {
            b.Property(x => x.Codigo).HasMaxLength(20);
            b.Property(x => x.Nombre).HasMaxLength(80);
            b.Property(x => x.Edificio).HasMaxLength(60);
            b.Property(x => x.Ubicacion).HasMaxLength(120);
            b.HasIndex(x => x.Codigo).IsUnique();
        });

        modelBuilder.Entity<ArticuloConocimiento>(b =>
        {
            b.Property(x => x.Titulo).HasMaxLength(200);
            b.Property(x => x.Etiquetas).HasMaxLength(300);
            b.Property(x => x.Referencias).HasMaxLength(1000);
            b.HasIndex(x => new { x.ServicioId, x.LaboratorioId });
            b.HasIndex(x => new { x.AutorId, x.Activo });
            b.HasIndex(x => x.IncidenciaOrigenId).HasDatabaseName("IX_KB_IncidenciaOrigen");
        });

        // Defaults de fecha (UTC)
        modelBuilder.Entity<Usuario>().Property(u => u.FechaRegistro).HasDefaultValueSql("SYSUTCDATETIME()");
        modelBuilder.Entity<Incidencia>().Property(i => i.FechaCreacion).HasDefaultValueSql("SYSUTCDATETIME()");
        modelBuilder.Entity<Incidencia>().Property(i => i.FechaUltimoCambio).HasDefaultValueSql("SYSUTCDATETIME()");
        modelBuilder.Entity<Seguimiento>().Property(s => s.Fecha).HasDefaultValueSql("SYSUTCDATETIME()");
        modelBuilder.Entity<ComentarioIncidencia>().Property(c => c.Fecha).HasDefaultValueSql("SYSUTCDATETIME()");
        modelBuilder.Entity<FeedbackIncidencia>().Property(f => f.Fecha).HasDefaultValueSql("SYSUTCDATETIME()");
        modelBuilder.Entity<Notificacion>().Property(n => n.Fecha).HasDefaultValueSql("SYSUTCDATETIME()");
        modelBuilder.Entity<SLA_Incidencia>().Property(s => s.CreadoUtc).HasDefaultValueSql("SYSUTCDATETIME()");
        modelBuilder.Entity<ArticuloConocimiento>().Property(a => a.FechaCreacion).HasDefaultValueSql("SYSUTCDATETIME()");
        modelBuilder.Entity<ArticuloConocimiento>().Property(a => a.UltimaActualizacion).HasDefaultValueSql("SYSUTCDATETIME()");

        // Relaciones explícitas (FKs visibles en DBeaver)
        // Servicio -> Categoria
        modelBuilder.Entity<Servicio>()
            .HasOne<Categoria>()
            .WithMany()
            .HasForeignKey(s => s.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Incidencia -> catálogos/usuarios/laboratorio
        modelBuilder.Entity<Incidencia>()
            .HasOne<EstadoIncidencia>()
            .WithMany()
            .HasForeignKey(i => i.EstadoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Incidencia>()
            .HasOne<Prioridad>()
            .WithMany()
            .HasForeignKey(i => i.PrioridadId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Incidencia>()
            .HasOne<Servicio>()
            .WithMany()
            .HasForeignKey(i => i.ServicioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Incidencia>()
            .HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(i => i.CreadorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Incidencia>()
            .HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(i => i.TecnicoAsignadoId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Incidencia>()
            .Property<int?>("LaboratorioId");
        modelBuilder.Entity<Incidencia>()
            .HasOne<Laboratorio>()
            .WithMany()
            .HasForeignKey("LaboratorioId")
            .OnDelete(DeleteBehavior.SetNull);

        // Seguimiento -> Incidencia/Estados/Usuario
        modelBuilder.Entity<Seguimiento>()
            .HasOne<Incidencia>()
            .WithMany()
            .HasForeignKey(s => s.IncidenciaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Seguimiento>()
            .HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(s => s.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Seguimiento>()
            .HasOne<EstadoIncidencia>()
            .WithMany()
            .HasForeignKey(s => s.EstadoAnteriorId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Seguimiento>()
            .HasOne<EstadoIncidencia>()
            .WithMany()
            .HasForeignKey(s => s.EstadoNuevoId)
            .OnDelete(DeleteBehavior.NoAction);

        // Comentario -> Incidencia/Usuario
        modelBuilder.Entity<ComentarioIncidencia>()
            .HasOne<Incidencia>()
            .WithMany()
            .HasForeignKey(c => c.IncidenciaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ComentarioIncidencia>()
            .HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Feedback -> Incidencia/Usuario
        modelBuilder.Entity<FeedbackIncidencia>()
            .HasOne<Incidencia>()
            .WithMany()
            .HasForeignKey(f => f.IncidenciaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FeedbackIncidencia>()
            .HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(f => f.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Notificacion -> Usuario destino
        modelBuilder.Entity<Notificacion>()
            .HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(n => n.UsuarioDestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        // SLA
        modelBuilder.Entity<SLA_Definicion>()
            .HasOne<Prioridad>()
            .WithMany()
            .HasForeignKey(d => d.PrioridadId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SLA_Definicion>()
            .HasOne<Servicio>()
            .WithMany()
            .HasForeignKey(d => d.ServicioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SLA_Incidencia>()
            .HasOne<Incidencia>()
            .WithMany()
            .HasForeignKey(s => s.IncidenciaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Escalacion -> Incidencia
        modelBuilder.Entity<Escalacion>()
            .HasOne<Incidencia>()
            .WithMany()
            .HasForeignKey(e => e.IncidenciaId)
            .OnDelete(DeleteBehavior.Cascade);

        // ArticuloConocimiento -> Servicio/Laboratorio/Autor/IncidenciaOrigen
        modelBuilder.Entity<ArticuloConocimiento>()
            .HasOne<Servicio>()
            .WithMany()
            .HasForeignKey(a => a.ServicioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ArticuloConocimiento>()
            .HasOne<Laboratorio>()
            .WithMany()
            .HasForeignKey(a => a.LaboratorioId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ArticuloConocimiento>()
            .HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(a => a.AutorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ArticuloConocimiento>()
            .HasOne<Incidencia>()
            .WithMany()
            .HasForeignKey(a => a.IncidenciaOrigenId)
            .OnDelete(DeleteBehavior.SetNull);

        // Seeds (roles/estados/prioridades/categorías/servicios/laboratorios/SLA/usuarios)
        // Roles
        modelBuilder.Entity<Rol>().HasData(
            new Rol { Id = 1, Nombre = "Estudiante" },
            new Rol { Id = 2, Nombre = "Tecnico" },
            new Rol { Id = 3, Nombre = "Administrador" },
            new Rol { Id = 4, Nombre = "Docente" }
        );

        // Estados de incidencia
        modelBuilder.Entity<EstadoIncidencia>().HasData(
            new EstadoIncidencia { Id = 1, Codigo = "ABIERTO",      EsFinal = false, Orden = 1 },
            new EstadoIncidencia { Id = 2, Codigo = "EN_PROGRESO",  EsFinal = false, Orden = 2 },
            new EstadoIncidencia { Id = 3, Codigo = "RESUELTO",     EsFinal = false, Orden = 3 },
            new EstadoIncidencia { Id = 4, Codigo = "CERRADO",      EsFinal = true,  Orden = 4 },
            new EstadoIncidencia { Id = 5, Codigo = "REABIERTO",    EsFinal = false, Orden = 5 }
        );

        // Prioridades
        modelBuilder.Entity<Prioridad>().HasData(
            new Prioridad { Id = 1, Nombre = "Baja",    Peso = 1, TiempoMaxHoras = 168 },
            new Prioridad { Id = 2, Nombre = "Media",   Peso = 2, TiempoMaxHoras = 72 },
            new Prioridad { Id = 3, Nombre = "Alta",    Peso = 3, TiempoMaxHoras = 24 },
            new Prioridad { Id = 4, Nombre = "Crítica", Peso = 4, TiempoMaxHoras = 4 }
        );

        // Categorías
        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nombre = "Hardware" },
            new Categoria { Id = 2, Nombre = "Software" },
            new Categoria { Id = 3, Nombre = "Red" },
            new Categoria { Id = 4, Nombre = "Acceso" },
            new Categoria { Id = 5, Nombre = "Audiovisual" },
            new Categoria { Id = 6, Nombre = "Climatización" },
            new Categoria { Id = 7, Nombre = "Mobiliario" },
            new Categoria { Id = 8, Nombre = "Otro" }
        );

        // Servicios
        modelBuilder.Entity<Servicio>().HasData(
            new Servicio { Id = 1, Nombre = "Soporte PC", CategoriaId = 1 },
            new Servicio { Id = 2, Nombre = "Red/Conectividad", CategoriaId = 3 },
            new Servicio { Id = 3, Nombre = "Software", CategoriaId = 2 },
            new Servicio { Id = 4, Nombre = "Audiovisual", CategoriaId = 5 }
        );

        // Laboratorios base
        modelBuilder.Entity<Laboratorio>().HasData(
            new Laboratorio { Id = 1, Codigo = "LAB-RED", Nombre = "Lab Redes", Edificio = "FISEI", Ubicacion = "Piso 2" },
            new Laboratorio { Id = 2, Codigo = "LAB-SW",  Nombre = "Lab Software", Edificio = "FISEI", Ubicacion = "Piso 3" },
            new Laboratorio { Id = 3, Codigo = "LAB-HW",  Nombre = "Lab Hardware", Edificio = "FISEI", Ubicacion = "Piso 1" },
            new Laboratorio { Id = 4, Codigo = "LAB-AV",  Nombre = "Lab Audiovisual", Edificio = "FISEI", Ubicacion = "Piso 4" }
        );

        // SLA por prioridad (ejemplos)
        modelBuilder.Entity<SLA_Definicion>().HasData(
            new SLA_Definicion { Id = 1, PrioridadId = 4, HorasRespuesta = 0,  HorasResolucion = 4 },  // 15m~0h resp, 4h resolución
            new SLA_Definicion { Id = 2, PrioridadId = 3, HorasRespuesta = 1,  HorasResolucion = 24 },
            new SLA_Definicion { Id = 3, PrioridadId = 2, HorasRespuesta = 4,  HorasResolucion = 72 },
            new SLA_Definicion { Id = 4, PrioridadId = 1, HorasRespuesta = 8,  HorasResolucion = 168 }
        );

        // Usuarios base (20) con hash/salt demo
        var hash = "7bLj4V/UqVZU/y/tYw/jheG+yr1QU1KyWjfOZaCKuKY=";
        var salt = "lOvvglGCDYWu2T3sCDzE1A==";
        var now = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario { Id=1,  Nombre="Admin 1",      Correo="admin1@admin.com",   PasswordHash=hash, PasswordSalt=salt, RolId=3, Activo=true, FechaRegistro=now },
            new Usuario { Id=2,  Nombre="Admin 2",      Correo="admin2@admin.com",   PasswordHash=hash, PasswordSalt=salt, RolId=3, Activo=true, FechaRegistro=now },
            new Usuario { Id=3,  Nombre="Tecnico 1",    Correo="tecnico1@uta.edu.ec", PasswordHash=hash, PasswordSalt=salt, RolId=2, Activo=true, FechaRegistro=now },
            new Usuario { Id=4,  Nombre="Tecnico 2",    Correo="tecnico2@uta.edu.ec", PasswordHash=hash, PasswordSalt=salt, RolId=2, Activo=true, FechaRegistro=now },
            new Usuario { Id=5,  Nombre="Tecnico 3",    Correo="tecnico3@uta.edu.ec", PasswordHash=hash, PasswordSalt=salt, RolId=2, Activo=true, FechaRegistro=now },
            new Usuario { Id=6,  Nombre="Tecnico 4",    Correo="tecnico4@uta.edu.ec", PasswordHash=hash, PasswordSalt=salt, RolId=2, Activo=true, FechaRegistro=now },
            new Usuario { Id=7,  Nombre="Docente 1",    Correo="docente1@uta.edu.ec", PasswordHash=hash, PasswordSalt=salt, RolId=4, Activo=true, FechaRegistro=now },
            new Usuario { Id=8,  Nombre="Docente 2",    Correo="docente2@uta.edu.ec", PasswordHash=hash, PasswordSalt=salt, RolId=4, Activo=true, FechaRegistro=now },
            new Usuario { Id=9,  Nombre="Docente 3",    Correo="docente3@uta.edu.ec", PasswordHash=hash, PasswordSalt=salt, RolId=4, Activo=true, FechaRegistro=now },
            new Usuario { Id=10, Nombre="Docente 4",    Correo="docente4@uta.edu.ec", PasswordHash=hash, PasswordSalt=salt, RolId=4, Activo=true, FechaRegistro=now },
            new Usuario { Id=11, Nombre="Estudiante 1", Correo="est1@uta.edu.ec",     PasswordHash=hash, PasswordSalt=salt, RolId=1, Activo=true, FechaRegistro=now },
            new Usuario { Id=12, Nombre="Estudiante 2", Correo="est2@uta.edu.ec",     PasswordHash=hash, PasswordSalt=salt, RolId=1, Activo=true, FechaRegistro=now },
            new Usuario { Id=13, Nombre="Estudiante 3", Correo="est3@uta.edu.ec",     PasswordHash=hash, PasswordSalt=salt, RolId=1, Activo=true, FechaRegistro=now },
            new Usuario { Id=14, Nombre="Estudiante 4", Correo="est4@uta.edu.ec",     PasswordHash=hash, PasswordSalt=salt, RolId=1, Activo=true, FechaRegistro=now },
            new Usuario { Id=15, Nombre="Estudiante 5", Correo="est5@uta.edu.ec",     PasswordHash=hash, PasswordSalt=salt, RolId=1, Activo=true, FechaRegistro=now },
            new Usuario { Id=16, Nombre="Estudiante 6", Correo="est6@uta.edu.ec",     PasswordHash=hash, PasswordSalt=salt, RolId=1, Activo=true, FechaRegistro=now },
            new Usuario { Id=17, Nombre="Estudiante 7", Correo="est7@uta.edu.ec",     PasswordHash=hash, PasswordSalt=salt, RolId=1, Activo=true, FechaRegistro=now },
            new Usuario { Id=18, Nombre="Estudiante 8", Correo="est8@uta.edu.ec",     PasswordHash=hash, PasswordSalt=salt, RolId=1, Activo=true, FechaRegistro=now },
            new Usuario { Id=19, Nombre="Estudiante 9", Correo="est9@uta.edu.ec",     PasswordHash=hash, PasswordSalt=salt, RolId=1, Activo=true, FechaRegistro=now },
            new Usuario { Id=20, Nombre="Estudiante 10",Correo="est10@uta.edu.ec",    PasswordHash=hash, PasswordSalt=salt, RolId=1, Activo=true, FechaRegistro=now }
        );

        base.OnModelCreating(modelBuilder);
    }
}