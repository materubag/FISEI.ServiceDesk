using Microsoft.EntityFrameworkCore;
using FISEI.ServiceDesk.Domain.Entities;

namespace FISEI.ServiceDesk.Infrastructure.Persistence;

public class ServiceDeskDbContext : DbContext
{
    public ServiceDeskDbContext(DbContextOptions<ServiceDeskDbContext> options) : base(options) {}

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Índices
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Correo)
            .IsUnique();

        modelBuilder.Entity<Incidencia>()
            .Property(i => i.Titulo)
            .HasMaxLength(200);

        modelBuilder.Entity<EstadoIncidencia>()
            .HasIndex(e => e.Codigo)
            .IsUnique();

        // Semillas (IDs fijos para consistencia en migraciones)
        var estudianteId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
        var tecnicoId    = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2");
        var adminId      = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc3");

        modelBuilder.Entity<Rol>().HasData(
            new Rol { Id = 1, Nombre = "Estudiante" },
            new Rol { Id = 2, Nombre = "Tecnico" },
            new Rol { Id = 3, Nombre = "Administrador" }
        );

        modelBuilder.Entity<Usuario>().HasData(
            new Usuario { Id = estudianteId, Nombre = "Estudiante Demo", Correo = "estudiante@demo.local", PasswordHash = "hash", RolId = 1, Activo = true },
            new Usuario { Id = tecnicoId,    Nombre = "Tecnico Demo",    Correo = "tecnico@demo.local",    PasswordHash = "hash", RolId = 2, Activo = true },
            new Usuario { Id = adminId,      Nombre = "Admin Demo",      Correo = "admin@demo.local",      PasswordHash = "hash", RolId = 3, Activo = true }
        );

        modelBuilder.Entity<EstadoIncidencia>().HasData(
            new EstadoIncidencia { Id = 1, Codigo = "REPORTADO",   EsFinal = false, Orden = 1 },
            new EstadoIncidencia { Id = 2, Codigo = "ASIGNADO",    EsFinal = false, Orden = 2 },
            new EstadoIncidencia { Id = 3, Codigo = "EN_PROCESO",  EsFinal = false, Orden = 3 },
            new EstadoIncidencia { Id = 4, Codigo = "RESUELTO",    EsFinal = false, Orden = 4 },
            new EstadoIncidencia { Id = 5, Codigo = "CERRADO",     EsFinal = true,  Orden = 5 }
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
            new Servicio { Id = 1, CategoriaId = 1, Nombre = "WiFi",        Activo = true },
            new Servicio { Id = 2, CategoriaId = 1, Nombre = "Ethernet",    Activo = true },
            new Servicio { Id = 3, CategoriaId = 2, Nombre = "Correo",      Activo = true },
            new Servicio { Id = 4, CategoriaId = 2, Nombre = "Impresoras",  Activo = true },
            new Servicio { Id = 5, CategoriaId = 3, Nombre = "AulaVirtual", Activo = true }
        );

        base.OnModelCreating(modelBuilder);
    }
}