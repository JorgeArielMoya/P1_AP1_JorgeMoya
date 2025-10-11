using Microsoft.EntityFrameworkCore;
using P1_AP1_JorgeMoya.Models;

namespace P1_AP1_JorgeMoya.DAL;

public class Contexto : DbContext
{
    public DbSet<EntradasHuacalesDetalle> EntradasHuacalesDetalles { get; set; } 
    public DbSet<EntradasHuacales> EntradasHuacales { get; set; }
    public DbSet<TiposHuacales> TiposHuacales { get; set; } 
    public Contexto(DbContextOptions<Contexto> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TiposHuacales>().HasData
        (
            new List<TiposHuacales>()
            {
                new ()
                {
                    TipoId = 1,
                    Descripcion = "Rojo",
                    Existencia = 0
                },

                new ()
                {
                    TipoId = 2,
                    Descripcion = "Verde",
                    Existencia = 0
                },

                new ()
                {
                    TipoId = 3,
                    Descripcion = "Negro",
                    Existencia = 0
                },

                new ()
                {
                    TipoId = 4,
                    Descripcion = "Azul",
                    Existencia = 0
                },

                new ()
                {
                    TipoId = 5,
                    Descripcion = "Amarillo",
                    Existencia = 0
                },

                new ()
                {
                    TipoId = 6,
                    Descripcion = "Naranja",
                    Existencia = 0
                }
            }
        );
        base.OnModelCreating(modelBuilder);
    }
}
