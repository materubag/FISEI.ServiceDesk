namespace FISEI.ServiceDesk.Domain.Entities;

public class Escalacion
{
    public int Id { get; set; } // IDENTITY
    public int IncidenciaId { get; set; }
    public int Nivel { get; set; }
    public string Motivo { get; set; } = default!;
    public DateTime Fecha { get; set; }
}