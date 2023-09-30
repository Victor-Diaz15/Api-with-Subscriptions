using Microsoft.AspNetCore.Identity;

namespace WebApiAutores.Entiities
{
    public class Comentario
    {
        public int Id { get; set; }
        public string Contenido { get; set; }

        //relations
        public int LibroId { get; set; }
        public Libro Libro { get; set; }

        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

    }
}
