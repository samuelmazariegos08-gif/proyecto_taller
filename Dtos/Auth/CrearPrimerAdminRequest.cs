using System.ComponentModel.DataAnnotations;

namespace TallerBackend.Dtos.Auth;

public class CrearPrimerAdminRequest
{
    [Required, MaxLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(120)]
    public string Correo { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;
}
