using System;

namespace FISEI.ServiceDesk.Domain.Entities;

public class Escalacion
{
    public long Id { get; set; }
    public long IncidenciaId { get; set; }
    public int Nivel { get; set; } 
    public string Motivo { get; set; } = default!;
    public DateTime Fecha { get; set; }
}