using System.ComponentModel.DataAnnotations;

namespace TallerBackend.Dtos.Usuarios;

public class ActualizarUsuarioRequest
{
    [Required, MaxLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(120)]
    public string Correo { get; set; } = string.Empty;

    [Required]
    public int IdRol { get; set; }

    public bool Activo { get; set; } = true;
}
