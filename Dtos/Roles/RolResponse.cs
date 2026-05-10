namespace TallerBackend.Dtos.Roles;

public class RolResponse
{
    public int IdRol { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
