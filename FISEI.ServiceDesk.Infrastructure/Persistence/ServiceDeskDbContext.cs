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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rol>().HasData(
            new Rol { Id = 1, Nombre = "Estudiante" },
            new Rol { Id = 2, Nombre = "Tecnico" },
            new Rol { Id = 3, Nombre = "Administrador" }
        );

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Correo)
            .IsUnique();

        modelBuilder.Entity<Incidencia>()
            .Property(i => i.Titulo)
            .HasMaxLength(200);

        base.OnModelCreating(modelBuilder);
    }
}