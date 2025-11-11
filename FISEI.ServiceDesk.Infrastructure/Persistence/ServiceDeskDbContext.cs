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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Índices y configuraciones simples
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Correo)
            .IsUnique();

        modelBuilder.Entity<Incidencia>()
            .Property(i => i.Titulo)
            .HasMaxLength(200);

        modelBuilder.Entity<EstadoIncidencia>()
            .HasIndex(e => e.Codigo)
            .IsUnique();

        modelBuilder.Entity<Usuario>()
            .Property(u => u.FechaRegistro)
            .HasDefaultValueSql("SYSUTCDATETIME()");
        modelBuilder.Entity<Incidencia>()
            .Property(i => i.FechaCreacion)
            .HasDefaultValueSql("SYSUTCDATETIME()");
        modelBuilder.Entity<Incidencia>()
            .Property(i => i.FechaUltimoCambio)
            .HasDefaultValueSql("SYSUTCDATETIME()");
        modelBuilder.Entity<Seguimiento>()
            .Property(s => s.Fecha)
            .HasDefaultValueSql("SYSUTCDATETIME()");
        modelBuilder.Entity<ComentarioIncidencia>()
            .Property(c => c.Fecha)
            .HasDefaultValueSql("SYSUTCDATETIME()");
        modelBuilder.Entity<FeedbackIncidencia>()
            .Property(f => f.Fecha)
            .HasDefaultValueSql("SYSUTCDATETIME()");
        modelBuilder.Entity<Notificacion>()
            .Property(n => n.Fecha)
            .HasDefaultValueSql("SYSUTCDATETIME()");
        modelBuilder.Entity<SLA_Incidencia>()
            .Property(s => s.CreadoUtc)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        var estudianteId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
        var tecnicoId    = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2");
        var adminId      = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc3");
        var seedDate     = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Rol>().HasData(
            new Rol { Id = 1, Nombre = "Estudiante" },
            new Rol { Id = 2, Nombre = "Tecnico" },
            new Rol { Id = 3, Nombre = "Administrador" }
        );

        // ÚNICO bloque de Usuario (reemplaza HASH_REAL/SALT_REAL por valores generados)
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario {
                Id = estudianteId,
                Nombre = "Estudiante Demo",
                Correo = "estudiante@demo.local",
                PasswordHash = "HASH_REAL",
                PasswordSalt = "SALT_REAL",
                RolId = 1,
                Activo = true,
                FechaRegistro = seedDate
            },
            new Usuario {
                Id = tecnicoId,
                Nombre = "Tecnico Demo",
                Correo = "tecnico@demo.local",
                PasswordHash = "HASH_REAL",
                PasswordSalt = "SALT_REAL",
                RolId = 2,
                Activo = true,
                FechaRegistro = seedDate
            },
            new Usuario {
                Id = adminId,
                Nombre = "Admin Demo",
                Correo = "admin@demo.local",
                PasswordHash = "HASH_REAL",
                PasswordSalt = "SALT_REAL",
                RolId = 3,
                Activo = true,
                FechaRegistro = seedDate
            }
        );

        modelBuilder.Entity<EstadoIncidencia>().HasData(
            new EstadoIncidencia { Id = 1, Codigo = "REPORTADO",  EsFinal = false, Orden = 1 },
            new EstadoIncidencia { Id = 2, Codigo = "ASIGNADO",   EsFinal = false, Orden = 2 },
            new EstadoIncidencia { Id = 3, Codigo = "EN_PROCESO", EsFinal = false, Orden = 3 },
            new EstadoIncidencia { Id = 4, Codigo = "RESUELTO",   EsFinal = false, Orden = 4 },
            new EstadoIncidencia { Id = 5, Codigo = "CERRADO",    EsFinal = true,  Orden = 5 }
        );

        modelBuilder.Entity<Prioridad>().HasData(
            new Prioridad { Id = 1, Nombre = "Baja",    Peso = 1, TiempoMaxHoras = 72 },
            new Prioridad { Id = 2, Nombre = "Media",   Peso = 2, TiempoMaxHoras = 48 },
            new Prioridad { Id = 3, Nombre = "Alta",    Peso = 3, TiempoMaxHoras = 24 },
            new Prioridad { Id = 4, Nombre = "Critica", Peso = 4, TiempoMaxHoras = 8 }
        );

        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nombre = "Redes",      Activo = true },
            new Categoria { Id = 2, Nombre = "Ofimática",  Activo = true },
            new Categoria { Id = 3, Nombre = "AulaVirtual",Activo = true }
        );

        modelBuilder.Entity<Servicio>().HasData(
            new Servicio { Id = 1, CategoriaId = 1, Nombre = "WiFi",       Activo = true },
            new Servicio { Id = 2, CategoriaId = 1, Nombre = "Ethernet",   Activo = true },
            new Servicio { Id = 3, CategoriaId = 2, Nombre = "Correo",     Activo = true },
            new Servicio { Id = 4, CategoriaId = 2, Nombre = "Impresoras", Activo = true },
            new Servicio { Id = 5, CategoriaId = 3, Nombre = "AulaVirtual",Activo = true }
        );

        modelBuilder.Entity<SLA_Definicion>().HasData(
            new SLA_Definicion { Id = 1, PrioridadId = 1, ServicioId = null, HorasRespuesta = 24, HorasResolucion = 72, Activo = true },
            new SLA_Definicion { Id = 2, PrioridadId = 2, ServicioId = null, HorasRespuesta = 12, HorasResolucion = 48, Activo = true },
            new SLA_Definicion { Id = 3, PrioridadId = 3, ServicioId = null, HorasRespuesta = 4,  HorasResolucion = 24, Activo = true },
            new SLA_Definicion { Id = 4, PrioridadId = 4, ServicioId = null, HorasRespuesta = 2,  HorasResolucion = 8,  Activo = true }
        );

        base.OnModelCreating(modelBuilder);
    }
}