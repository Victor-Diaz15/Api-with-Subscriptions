namespace WebApiAutores.DTOs
{
    public class DataHATEOAS
    {
        public string Enlance { get; private set; }
        public string Descripcion { get; private set; }
        public string Metodo { get; private set; }

        public DataHATEOAS(string enlace, string descripcion, string metodo)
        {
            Enlance = enlace;
            Descripcion = descripcion;
            Metodo = metodo;
        }
    }
}
