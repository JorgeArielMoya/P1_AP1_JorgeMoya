using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P1_AP1_JorgeMoya.Models;

public class EntradasHuacales
{
    [Key]
    public int IdEntrada { get; set; }

    [Required(ErrorMessage = "Campo Requerido")]
    public string NombreCliente { get; set; } = null!;

    [Required(ErrorMessage = "Campo Requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "Cantidad no valida")]
    public int Cantidad {  get; set; }

    [Required(ErrorMessage = "Campo Requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Precio no valido")]
    public double Precio { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    public virtual ICollection<EntradasHuacalesDetalle> EntradasHuacalesDetalles { get; set; } = new List<EntradasHuacalesDetalle>();
}
