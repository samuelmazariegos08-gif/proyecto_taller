using System.ComponentModel.DataAnnotations;

namespace TallerBackend.Dtos.Auth;

public class LoginRequest
{
    [Required]
    public string UsuarioOCorreo { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
