namespace TallerBackend.Dtos.Vehiculos;

public class VehiculoResponse
{
    public int IdVehiculo { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public short Anio { get; set; }
    public int Kilometraje { get; set; }
    public bool Activo { get; set; }
}
