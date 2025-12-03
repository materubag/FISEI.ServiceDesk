namespace FISEI.ServiceDesk.Domain.Entities;

public class SLA_Definicion
{
    public int Id { get; set; } // ya era int
    public int PrioridadId { get; set; }
    public int? ServicioId { get; set; }
    public int HorasRespuesta { get; set; }
    public int HorasResolucion { get; set; }
    public bool Activo { get; set; } = true;
}