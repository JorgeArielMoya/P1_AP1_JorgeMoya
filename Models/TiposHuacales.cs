using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P1_AP1_JorgeMoya.Models;

public class TiposHuacales
{
    [Key]
    public int TipoId { get; set; }

    [Required(ErrorMessage = "Campo requerido")]
    public string Descripcion { get; set; } = null!;

    [Required(ErrorMessage = "Campo requerido")]
    [Range(0, int.MaxValue, ErrorMessage = "Existencia no valida")]
    public int Existencia { get; set; }

    public virtual ICollection<EntradasHuacalesDetalle> EntradasHuacalesDetalles { get; set; } = new List<EntradasHuacalesDetalle>();
}
