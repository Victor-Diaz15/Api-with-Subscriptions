using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using WebApiAutores.DTOs;
using WebApiAutores.Entiities;

namespace WebApiAutores.Controllers.V1
{
    [ApiController()]
    [Route("api/v1/restriccionesip")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class RestriccionIPController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;

        public RestriccionIPController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> Post(RestriccionIPCreateDto iPCreateDto)
        {
            var llaveDb = await _context.LlavesApi.FirstOrDefaultAsync(x => x.Id == iPCreateDto.LlaveId);

            if (llaveDb == null) return NotFound();

            var usuarioId = GetUsuarioId();

            if (llaveDb.UsuarioId != usuarioId) return Forbid();

            RestriccionIP Ip = mapper.Map<RestriccionIP>(iPCreateDto);

            _context.RestriccionesIP.Add(Ip);
            await _context.SaveChangesAsync();

            return NoContent();

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] RestriccionIPUpdateDto IpUpdateDto)
        {
            var restriccionDb = await _context.RestriccionesIP
                .Include(x => x.LlaveApi)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (restriccionDb == null) return NotFound();

            var usuarioId = GetUsuarioId();

            if (restriccionDb.LlaveApi.UsuarioId != usuarioId) return Forbid();

            var updatedIp = mapper.Map(IpUpdateDto, restriccionDb);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var restriccionDb = await _context.RestriccionesIP
                .Include(x => x.LlaveApi)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (restriccionDb == null) return NotFound();

            var usuarioId = GetUsuarioId();

            if (restriccionDb.LlaveApi.UsuarioId != usuarioId) return Forbid();

            _context.RestriccionesIP.Remove(restriccionDb);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
