using WebApiAutores.Entiities;

namespace WebApiAutores.Services
{
    public class LlaveApiService
    {
        private readonly ApplicationDbContext _context;

        public LlaveApiService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateLlave(string usuarioId, TipoLlave tipoLlave)
        {
            var llave = GenerarApiKey();

            var llaveApi = new LlaveApi()
            {
                Activa = true,
                Llave = llave,
                TipoLlave = tipoLlave,
                UsuarioId = usuarioId
            };

            _context.Add(llaveApi);
            await _context.SaveChangesAsync();
        }

        public string GenerarApiKey()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
