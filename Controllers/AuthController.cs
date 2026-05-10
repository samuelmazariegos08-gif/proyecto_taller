using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerBackend.Data;
using TallerBackend.Dtos.Auth;
using TallerBackend.Mapping;
using TallerBackend.Models;
using TallerBackend.Services;

namespace TallerBackend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly TallerDbContext _context;
    private readonly JwtTokenService _jwtTokenService;

    public AuthController(TallerDbContext context, JwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var usuarioBuscado = request.UsuarioOCorreo.Trim().ToLower();

        var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u =>
                u.Username.ToLower() == usuarioBuscado ||
                u.Correo.ToLower() == usuarioBuscado);

        if (usuario is null || !usuario.Activo || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
        {
            return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos." });
        }

        var token = _jwtTokenService.GenerarToken(usuario);

        return Ok(new AuthResponse
        {
            Token = token.Token,
            ExpiraEn = token.ExpiraEn,
            Usuario = usuario.ToResponse()
        });
    }

    [HttpPost("primer-admin")]
    public async Task<ActionResult<AuthResponse>> CrearPrimerAdmin(CrearPrimerAdminRequest request)
    {
        if (await _context.Usuarios.AnyAsync())
        {
            return Conflict(new { mensaje = "Ya existe al menos un usuario registrado." });
        }

        var rolAdmin = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Administrador");
        if (rolAdmin is null)
        {
            return BadRequest(new { mensaje = "No existe el rol Administrador en la base de datos." });
        }

        var usuario = new Usuario
        {
            Nombre = request.Nombre.Trim(),
            Username = request.Username.Trim(),
            Correo = request.Correo.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Activo = true,
            IdRol = rolAdmin.IdRol
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        usuario.Rol = rolAdmin;
        var token = _jwtTokenService.GenerarToken(usuario);

        return Created(string.Empty, new AuthResponse
        {
            Token = token.Token,
            ExpiraEn = token.ExpiraEn,
            Usuario = usuario.ToResponse()
        });
    }
}
