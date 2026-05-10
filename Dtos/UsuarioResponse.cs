namespace TallerBackend.Dtos;

public class UsuarioResponse
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public int IdRol { get; set; }
    public string Rol { get; set; } = string.Empty;
}
