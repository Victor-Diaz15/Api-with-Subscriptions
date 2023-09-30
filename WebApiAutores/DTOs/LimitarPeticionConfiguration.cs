namespace WebApiAutores.DTOs
{
    public class LimitarPeticionConfiguration
    {
        public int CantidadPeticionGratuitaPorDia { get; set; }
        public string[] ListaBlancaRutas { get; set; }
    }
}
