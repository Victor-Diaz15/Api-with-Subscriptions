using System.ComponentModel.DataAnnotations;
using WebApiAutores.Entiities;

namespace WebApiAutores.DTOs
{
    public class LibroCreacionDto
    {
        [Required]
        [StringLength(maximumLength: 250)]
        public string Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public List<int> AutoresIds { get; set; }
    }
}
