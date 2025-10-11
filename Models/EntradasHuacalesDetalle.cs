using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P1_AP1_JorgeMoya.Models;

public class EntradasHuacalesDetalle
{
    [Key]
    public int DetalleId { get; set; }
    public int IdEntrada { get; set; }
    public int TipoId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Cantidad no valida")]
    public int Cantidad { get; set; }
    [Range(0.01, double.MaxValue, ErrorMessage = "Precio no valida")]
    public double Precio { get; set; }

    [ForeignKey("IdEntrada")]
    public virtual EntradasHuacales EntradaHuacal { get; set; }

    [ForeignKey("TipoId")]
    public virtual TiposHuacales TiposHuacales { get; set; } 
}
