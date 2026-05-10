using Microsoft.EntityFrameworkCore;
using TallerBackend.Models;

namespace TallerBackend.Data;

public class TallerDbContext : DbContext
{
    public TallerDbContext(DbContextOptions<TallerDbContext> options) : base(options)
    {
    }

    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("rol");
            entity.HasKey(e => e.IdRol);

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(255);
            entity.Property(e => e.CreadoEn).HasColumnName("creado_en");

            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuario");
            entity.HasKey(e => e.IdUsuario);

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(120).IsRequired();
            entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Correo).HasColumnName("correo").HasMaxLength(120).IsRequired();
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.CreadoEn).HasColumnName("creado_en");
            entity.Property(e => e.ActualizadoEn).HasColumnName("actualizado_en");

            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Correo).IsUnique();

            entity.HasOne(e => e.Rol)
                .WithMany(e => e.Usuarios)
                .HasForeignKey(e => e.IdRol)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.ToTable("vehiculo");
            entity.HasKey(e => e.IdVehiculo);

            entity.Property(e => e.IdVehiculo).HasColumnName("id_vehiculo");
            entity.Property(e => e.Placa).HasColumnName("placa").HasMaxLength(20).IsRequired();
            entity.Property(e => e.Marca).HasColumnName("marca").HasMaxLength(80).IsRequired();
            entity.Property(e => e.Modelo).HasColumnName("modelo").HasMaxLength(80).IsRequired();
            entity.Property(e => e.Anio).HasColumnName("anio");
            entity.Property(e => e.Kilometraje).HasColumnName("kilometraje");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.CreadoEn).HasColumnName("creado_en");
            entity.Property(e => e.ActualizadoEn).HasColumnName("actualizado_en");

            entity.HasIndex(e => e.Placa).IsUnique();
        });
    }
}
