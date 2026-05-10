using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerBackend.Data;
using TallerBackend.Dtos.Vehiculos;
using TallerBackend.Mapping;
using TallerBackend.Models;

namespace TallerBackend.Controllers;

[ApiController]
[Route("api/vehiculos")]
[Authorize]
public class VehiculosController : ControllerBase
{
    private readonly TallerDbContext _context;

    public VehiculosController(TallerDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehiculoResponse>>> ObtenerVehiculos(
        [FromQuery] bool? activo,
        [FromQuery] string? buscar)
    {
        var query = _context.Vehiculos.AsNoTracking();

        if (activo.HasValue)
        {
            query = query.Where(v => v.Activo == activo.Value);
        }

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var texto = buscar.Trim().ToLower();
            query = query.Where(v =>
                v.Placa.ToLower().Contains(texto) ||
                v.Marca.ToLower().Contains(texto) ||
                v.Modelo.ToLower().Contains(texto));
        }

        var vehiculos = await query
            .OrderBy(v => v.Placa)
            .ToListAsync();

        return Ok(vehiculos.Select(v => v.ToResponse()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VehiculoResponse>> ObtenerVehiculo(int id)
    {
        var vehiculo = await _context.Vehiculos
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.IdVehiculo == id);

        return vehiculo is null
            ? NotFound(new { mensaje = "Vehículo no encontrado." })
            : Ok(vehiculo.ToResponse());
    }

    [HttpGet("placa/{placa}")]
    public async Task<ActionResult<VehiculoResponse>> ObtenerVehiculoPorPlaca(string placa)
    {
        var placaNormalizada = NormalizarPlaca(placa);
        var vehiculo = await _context.Vehiculos
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Placa == placaNormalizada);

        return vehiculo is null
            ? NotFound(new { mensaje = "Vehículo no encontrado." })
            : Ok(vehiculo.ToResponse());
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<VehiculoResponse>> CrearVehiculo(CrearVehiculoRequest request)
    {
        var placa = NormalizarPlaca(request.Placa);

        if (await _context.Vehiculos.AnyAsync(v => v.Placa == placa))
        {
            return Conflict(new { mensaje = "Ya existe un vehículo registrado con esa placa." });
        }

        var vehiculo = new Vehiculo
        {
            Placa = placa,
            Marca = request.Marca.Trim(),
            Modelo = request.Modelo.Trim(),
            Anio = request.Anio,
            Kilometraje = request.Kilometraje,
            Activo = request.Activo
        };

        _context.Vehiculos.Add(vehiculo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObtenerVehiculo), new { id = vehiculo.IdVehiculo }, vehiculo.ToResponse());
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<VehiculoResponse>> ActualizarVehiculo(int id, ActualizarVehiculoRequest request)
    {
        var vehiculo = await _context.Vehiculos.FindAsync(id);
        if (vehiculo is null)
        {
            return NotFound(new { mensaje = "Vehículo no encontrado." });
        }

        var placa = NormalizarPlaca(request.Placa);
        if (await _context.Vehiculos.AnyAsync(v => v.IdVehiculo != id && v.Placa == placa))
        {
            return Conflict(new { mensaje = "Ya existe otro vehículo registrado con esa placa." });
        }

        vehiculo.Placa = placa;
        vehiculo.Marca = request.Marca.Trim();
        vehiculo.Modelo = request.Modelo.Trim();
        vehiculo.Anio = request.Anio;
        vehiculo.Kilometraje = request.Kilometraje;
        vehiculo.Activo = request.Activo;

        await _context.SaveChangesAsync();

        return Ok(vehiculo.ToResponse());
    }

    [HttpPatch("{id:int}/estado")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CambiarEstado(int id, [FromQuery] bool activo)
    {
        var vehiculo = await _context.Vehiculos.FindAsync(id);
        if (vehiculo is null)
        {
            return NotFound(new { mensaje = "Vehículo no encontrado." });
        }

        vehiculo.Activo = activo;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static string NormalizarPlaca(string placa)
    {
        return placa.Trim().ToUpperInvariant();
    }
}
