namespace FISEI.ServiceDesk.Domain.Entities;

public class ArticuloConocimiento
{
    public int Id { get; set; }
    public string Titulo { get; set; } = default!;            // Resumen del problema/solución
    public string Contenido { get; set; } = default!;         // Detalle de la solución (markdown/texto)
    public string? Referencias { get; set; }                  // URLs, números de guía, etc.
    public string? Etiquetas { get; set; }                    // "wifi; impresora; drivers"
    public int? ServicioId { get; set; }                      // Relación con Servicio (opcional)
    public int? LaboratorioId { get; set; }                   // Relación con Laboratorio/Aula (opcional)
    public int AutorId { get; set; }                          // Técnico que registró
    public int? IncidenciaOrigenId { get; set; }              // Incidencia desde la cual se documentó (opcional)
    public DateTime FechaCreacion { get; set; }
    public DateTime UltimaActualizacion { get; set; }
    public bool Activo { get; set; } = true;
}