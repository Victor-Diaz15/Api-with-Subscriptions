namespace WebApiAutores.Entiities
{
    public class RestriccionDominio
    {
        public int Id { get; set; }
        public int LlaveApiId { get; set; }
        public string Dominio { get; set; }
        public LlaveApi LlaveApi { get; set; }
    }
}
