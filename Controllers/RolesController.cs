using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerBackend.Data;
using TallerBackend.Dtos.Roles;

namespace TallerBackend.Controllers;

[ApiController]
[Route("api/roles")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly TallerDbContext _context;

    public RolesController(TallerDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RolResponse>>> ObtenerRoles()
    {
        var roles = await _context.Roles
            .OrderBy(r => r.IdRol)
            .Select(r => new RolResponse
            {
                IdRol = r.IdRol,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion
            })
            .ToListAsync();

        return Ok(roles);
    }
}
