using System;

namespace FISEI.ServiceDesk.Domain.Entities;

public class ComentarioIncidencia
{
    public long Id { get; set; }
    public long IncidenciaId { get; set; }
    public Guid UsuarioId { get; set; }
    public string Texto { get; set; } = default!;
    public bool EsInterno { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}