namespace WebApiAutores.Entiities
{
    public class Peticion
    {
        public int Id { get; set; }
        public int LlaveId { get; set; }
        public DateTime FechaPeticion { get; set; }
        public LlaveApi Llave { get; set; }
    }
}
