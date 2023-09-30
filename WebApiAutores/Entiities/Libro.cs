using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validations;

namespace WebApiAutores.Entiities
{
    public class Libro
    {
        public int Id { get; set; }
        [Required]
        [PrimeraLetraMayuscula]
        public string Titulo { get; set; }
        public DateTime? FechaPublicacion { get; set; }

        //relations
        public List<Comentario> Comentarios { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; }

        //Foreign Key
        //public int AutorId { get; set; }

        //Property Navigation
        //public Autor Autor { get; set; }
    }
}
