namespace FISEI.ServiceDesk.Domain.Entities;

public class Prioridad
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public int Peso { get; set; }
    public int TiempoMaxHoras { get; set; } // SLA base
}