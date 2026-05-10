using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerBackend.Data;
using TallerBackend.Dtos;
using TallerBackend.Dtos.Usuarios;
using TallerBackend.Mapping;
using TallerBackend.Models;

namespace TallerBackend.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize(Roles = "Administrador")]
public class UsuariosController : ControllerBase
{
    private readonly TallerDbContext _context;

    public UsuariosController(TallerDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioResponse>>> ObtenerUsuarios()
    {
        var usuarios = await _context.Usuarios
            .Include(u => u.Rol)
            .OrderBy(u => u.IdUsuario)
            .Select(u => u.ToResponse())
            .ToListAsync();

        return Ok(usuarios);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioResponse>> ObtenerUsuario(int id)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.IdUsuario == id);

        return usuario is null
            ? NotFound(new { mensaje = "Usuario no encontrado." })
            : Ok(usuario.ToResponse());
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioResponse>> CrearUsuario(CrearUsuarioRequest request)
    {
        if (!await _context.Roles.AnyAsync(r => r.IdRol == request.IdRol))
        {
            return BadRequest(new { mensaje = "El rol indicado no existe." });
        }

        if (await _context.Usuarios.AnyAsync(u => u.Username == request.Username || u.Correo == request.Correo))
        {
            return Conflict(new { mensaje = "El username o correo ya está registrado." });
        }

        var usuario = new Usuario
        {
            Nombre = request.Nombre.Trim(),
            Username = request.Username.Trim(),
            Correo = request.Correo.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Activo = request.Activo,
            IdRol = request.IdRol
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        await _context.Entry(usuario).Reference(u => u.Rol).LoadAsync();

        return CreatedAtAction(nameof(ObtenerUsuario), new { id = usuario.IdUsuario }, usuario.ToResponse());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UsuarioResponse>> ActualizarUsuario(int id, ActualizarUsuarioRequest request)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.IdUsuario == id);

        if (usuario is null)
        {
            return NotFound(new { mensaje = "Usuario no encontrado." });
        }

        if (!await _context.Roles.AnyAsync(r => r.IdRol == request.IdRol))
        {
            return BadRequest(new { mensaje = "El rol indicado no existe." });
        }

        if (await _context.Usuarios.AnyAsync(u => u.IdUsuario != id && u.Correo == request.Correo))
        {
            return Conflict(new { mensaje = "El correo ya está registrado por otro usuario." });
        }

        usuario.Nombre = request.Nombre.Trim();
        usuario.Correo = request.Correo.Trim();
        usuario.IdRol = request.IdRol;
        usuario.Activo = request.Activo;

        await _context.SaveChangesAsync();
        await _context.Entry(usuario).Reference(u => u.Rol).LoadAsync();

        return Ok(usuario.ToResponse());
    }

    [HttpPatch("{id:int}/password")]
    public async Task<IActionResult> CambiarPassword(int id, CambiarPasswordRequest request)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario is null)
        {
            return NotFound(new { mensaje = "Usuario no encontrado." });
        }

        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id:int}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, [FromQuery] bool activo)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario is null)
        {
            return NotFound(new { mensaje = "Usuario no encontrado." });
        }

        usuario.Activo = activo;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
