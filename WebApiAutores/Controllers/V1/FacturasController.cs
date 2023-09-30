using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/facturas")]
    public class FacturasController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public FacturasController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Pagar(PagarFacturaDto pagarFacturaDto)
        {
            var facturaDb = await context.Facturas
                .Include(x => x.Usuario)
                .FirstOrDefaultAsync(x => x.Id == pagarFacturaDto.FacturaId);

            if (facturaDb == null) return NotFound();

            if (facturaDb.Pagada) return BadRequest("La factura ya fue saldada");

            //Logica de pagar la factura
            facturaDb.Pagada = true;
            await context.SaveChangesAsync();

            var hayFacturasVencidas = await context.Facturas
                .AnyAsync(x => x.UsuarioId == facturaDb.UsuarioId &&
                !x.Pagada && x.FechaLimiteDePago < DateTime.Today);

            if (!hayFacturasVencidas)
            {
                facturaDb.Usuario.MalaPaga = false;
                await context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
