using System.ComponentModel.DataAnnotations;

namespace TallerBackend.Dtos.Usuarios;

public class CambiarPasswordRequest
{
    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;
}
