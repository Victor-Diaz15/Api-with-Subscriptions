using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entiities;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/restriccionesdominio")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RestriccionDominioController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;

        public RestriccionDominioController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> Post(RestriccionDominioCreateDto dominioCreateDto)
        {
            var llaveDb = await _context.LlavesApi.FirstOrDefaultAsync(x => x.Id == dominioCreateDto.LlaveApiId);

            if (llaveDb == null) return NotFound();

            var usuarioId = GetUsuarioId();

            if (llaveDb.UsuarioId != usuarioId) return Forbid();

            RestriccionDominio dominio = mapper.Map<RestriccionDominio>(dominioCreateDto);

            _context.RestriccionesDominio.Add(dominio);
            await _context.SaveChangesAsync();

            return NoContent();

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] RestriccionDominioUpdateDto dominioUpdateDto)
        {
            var restriccionDb = await _context.RestriccionesDominio
                .Include(x => x.LlaveApi)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (restriccionDb == null) return NotFound();

            var usuarioId = GetUsuarioId();

            if (restriccionDb.LlaveApi.UsuarioId != usuarioId) return Forbid();

            var updatedDominio = mapper.Map(dominioUpdateDto, restriccionDb);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var restriccionDb = await _context.RestriccionesDominio
                .Include(x => x.LlaveApi)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (restriccionDb == null) return NotFound();

            var usuarioId = GetUsuarioId();

            if (restriccionDb.LlaveApi.UsuarioId != usuarioId) return Forbid();

            _context.RestriccionesDominio.Remove(restriccionDb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
