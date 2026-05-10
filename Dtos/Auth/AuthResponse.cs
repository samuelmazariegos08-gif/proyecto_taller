using TallerBackend.Dtos;

namespace TallerBackend.Dtos.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEn { get; set; }
    public UsuarioResponse Usuario { get; set; } = new();
}
