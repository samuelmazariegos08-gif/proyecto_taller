using System.ComponentModel.DataAnnotations;

namespace TallerBackend.Dtos.Vehiculos;

public class ActualizarVehiculoRequest
{
    [Required, MaxLength(20)]
    public string Placa { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string Marca { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string Modelo { get; set; } = string.Empty;

    [Range(1900, 2100)]
    public short Anio { get; set; }

    [Range(0, int.MaxValue)]
    public int Kilometraje { get; set; }

    public bool Activo { get; set; } = true;
}
