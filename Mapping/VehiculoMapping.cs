using TallerBackend.Dtos.Vehiculos;
using TallerBackend.Models;

namespace TallerBackend.Mapping;

public static class VehiculoMapping
{
    public static VehiculoResponse ToResponse(this Vehiculo vehiculo)
    {
        return new VehiculoResponse
        {
            IdVehiculo = vehiculo.IdVehiculo,
            Placa = vehiculo.Placa,
            Marca = vehiculo.Marca,
            Modelo = vehiculo.Modelo,
            Anio = vehiculo.Anio,
            Kilometraje = vehiculo.Kilometraje,
            Activo = vehiculo.Activo
        };
    }
}
