using TallerBackend.Dtos;
using TallerBackend.Models;

namespace TallerBackend.Mapping;

public static class UsuarioMapping
{
    public static UsuarioResponse ToResponse(this Usuario usuario)
    {
        return new UsuarioResponse
        {
            IdUsuario = usuario.IdUsuario,
            Nombre = usuario.Nombre,
            Username = usuario.Username,
            Correo = usuario.Correo,
            Activo = usuario.Activo,
            IdRol = usuario.IdRol,
            Rol = usuario.Rol?.Nombre ?? string.Empty
        };
    }
}
