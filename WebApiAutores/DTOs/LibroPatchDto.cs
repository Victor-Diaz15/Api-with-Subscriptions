using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOs
{
    public class LibroPatchDto
    {
        [Required]
        [StringLength(maximumLength: 250)]
        public string Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }
    }
}
