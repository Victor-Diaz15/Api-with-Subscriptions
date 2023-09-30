using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entiities;
using WebApiAutores.Services;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/llavesapi")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LlaveApiController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly LlaveApiService llaveApiService;

        public LlaveApiController(ApplicationDbContext context, IMapper mapper,
            LlaveApiService llaveApiService)
        {
            this.context = context;
            this.mapper = mapper;
            this.llaveApiService = llaveApiService;
        }

        [HttpGet]
        public async Task<ActionResult<List<LlaveApiDto>>> GetMyApiKey()
        {
            try
            {
                var usuarioId = GetUsuarioId();

                var apiKeys = await context.LlavesApi
                    .Include(x => x.RestriccionesDominio)
                    .Include(x => x.RestriccionesIP)
                    .Where(x => x.UsuarioId == usuarioId).ToListAsync();

                var apiKeysDto = mapper.Map<List<LlaveApiDto>>(apiKeys);

                return Ok(apiKeysDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateApiKey(LlaveApiCreateDto llaveApiCreateDto)
        {
            try
            {
                var usuarioId = GetUsuarioId();

                if(llaveApiCreateDto.TipoLlave == TipoLlave.Gratuita)
                {
                    return BadRequest("El usuario ya tiene una api key gratuita");
                }


                await llaveApiService.CreateLlave(usuarioId, llaveApiCreateDto.TipoLlave);

                return NoContent();

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> UpdateApiKey(LlaveApiUpdateDto llaveApiUpdateDto)
        {
            try
            {
                var usuarioId = GetUsuarioId();

                var llaveDb = await context.LlavesApi.FirstOrDefaultAsync(x => x.Id == llaveApiUpdateDto.LlaveId);

                if(llaveDb == null) return NotFound();

                if (usuarioId != llaveDb.UsuarioId) return Forbid();

                if (llaveApiUpdateDto.ActualizarLlave)
                    llaveDb.Llave = llaveApiService.GenerarApiKey();

                llaveDb.Activa = llaveApiUpdateDto.Activa;
                await context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
