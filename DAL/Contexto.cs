using Microsoft.EntityFrameworkCore;
using P1_AP1_JorgeMoya.Models;

namespace P1_AP1_JorgeMoya.DAL;

public class Contexto : DbContext
{
    public DbSet<EntradasHuacales> Registros { get; set; }

    public Contexto(DbContextOptions<Contexto> options) : base(options) { }
}
