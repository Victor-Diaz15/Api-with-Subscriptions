namespace WebApiAutores.Entiities
{
    public class AutorLibro
    {
        public int LibroId { get; set; }
        public int AutorId { get; set; }
        public int Orden { get; set; }

        //Navigations properties
        public Libro Libro { get; set; }
        public Autor Autor { get; set; }
    }
}
