using System.ComponentModel.DataAnnotations;

namespace TallerBackend.Dtos.Usuarios;

public class CrearUsuarioRequest
{
    [Required, MaxLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(120)]
    public string Correo { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public int IdRol { get; set; }

    public bool Activo { get; set; } = true;
}
