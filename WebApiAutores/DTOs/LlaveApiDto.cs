using WebApiAutores.Entiities;

namespace WebApiAutores.DTOs
{
    public class LlaveApiDto
    {
        public int Id { get; set; }
        public string Llave { get; set; }
        public bool Activa { get; set; }
        public string TipoLlave { get; set; }
        public List<RestriccionDominioDto> RestriccionesDominio { get; set; }
        public List<RestriccionIPDto> RestriccionesIP { get; set; }
    }
}
