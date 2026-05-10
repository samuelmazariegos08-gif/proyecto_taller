namespace TallerBackend.Models;

public class Usuario
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public int IdRol { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime ActualizadoEn { get; set; }

    public Rol? Rol { get; set; }
}
