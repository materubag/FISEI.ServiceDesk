using System;

namespace FISEI.ServiceDesk.Application.DTOs.Comentarios;

public class CreateComentarioDto
{
    public Guid UsuarioId { get; set; }
    public string Texto { get; set; } = default!;
    public bool EsInterno { get; set; }
}