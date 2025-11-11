using System;

namespace FISEI.ServiceDesk.Domain.Entities;

public class Notificacion
{
    public long Id { get; set; }
    public Guid UsuarioDestinoId { get; set; }
    public string Tipo { get; set; } = default!; // NUEVA_INCIDENCIA, ESTADO_CAMBIADO, NUEVO_COMENTARIO
    public string? Referencia { get; set; } // INC-000123
    public string Mensaje { get; set; } = default!;
    public bool Leida { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}