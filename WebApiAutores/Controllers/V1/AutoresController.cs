using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entiities;
using WebApiAutores.Filters;
using WebApiAutores.Utilidades;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    //[Route("api/v1/autores")]    
    [Route("api/autores")]
    [CabeceraEstaPresente("x-version", "1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class AutoresController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IAuthorizationService authorizationService;

        //private readonly ServicioTransient servicioTransient;
        //private readonly ServicioScoped servicioScoped;
        //private readonly ServicioSingleton servicioSingleton;
        //private readonly IServicio servicio;
        //private readonly ILogger logger;

        public ApplicationDbContext _context { get; }

        public AutoresController(ApplicationDbContext context, IMapper mapper,
            IConfiguration configuration, IAuthorizationService authorizationService
            //ServicioTransient servicioTransient, ServicioScoped servicioScoped,
            //ServicioSingleton servicioSingleton,
            //IServicio servicio, ILogger<AutoresController> logger
            )
        {
            _context = context;
            this.mapper = mapper;
            this.configuration = configuration;
            this.authorizationService = authorizationService;
            //this.servicioTransient = servicioTransient;
            //this.servicioScoped = servicioScoped;
            //this.servicioSingleton = servicioSingleton;
            //this.servicio = servicio;
            //this.logger = logger;
        }


        //Ejemplo para demostrar las diferentes formas y comportamientos de las inyecciones de dependencias.

        //[HttpGet("GUID")]
        ////[ResponseCache(Duration = 10)]
        //[ServiceFilter(typeof(MyFilterOfAction))]
        //public ActionResult ObtenerGuid()
        //{
        //    return Ok(new
        //    {
        //        AutoresController_Transient = servicioTransient.guid,
        //        ServicioA_Transient = servicio.ObtenerTransient(),
        //        AutoresController_Scoped = servicioScoped.guid,
        //        ServicioA_Scoped = servicio.ObtenerScoped(),
        //        AutoresController_Singleton = servicioSingleton.guid,
        //        ServicioA_Singleton = servicio.ObtenerSingleton()
        //    });
        //}

        //Ejemplo de como utilizar el archivo de configuratio
        //[HttpGet("configuraciones")]
        //public ActionResult<string> config()
        //{
        //    return configuration["ConnectionStrings:defaultConnection"];
        //}

        [HttpGet(Name = "obtenerAutoresv1")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]

        public async Task<ActionResult<List<AutorDto>>> Get([FromQuery] PaginacionDto paginacionDto)
        {
            var queryable = _context.Autores.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);

            var autores = await _context.Autores
                .OrderBy(x => x.Nombre)
                .Paginar(paginacionDto)
                .ToListAsync();

            return mapper.Map<List<AutorDto>>(autores);

        }

        [HttpGet("{id:int}", Name = "ObtenerAutorv1")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<AutorConLibrosDto>> Get(int id)
        {
            var autor = await _context.Autores
                .Include(x => x.AutoresLibros)
                .ThenInclude(x => x.Libro)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            var autoResponse = mapper.Map<AutorConLibrosDto>(autor);

            return Ok(autoResponse);
        }


        [HttpGet("{nombre}", Name = "obtenerAutorPorNombrev1")]
        public async Task<ActionResult<List<AutorDto>>> GetByName(string nombre)
        {
            var autor = await _context.Autores.Where
                (x => x.Nombre.Contains(nombre))
                .ToListAsync();

            return Ok(mapper.Map<List<AutorDto>>(autor));
        }

        [HttpPost(Name = "crearAutorv1")]
        public async Task<ActionResult> Post(AutorDto autorDto)
        {

            var existeAutorConElMismoNombre = await _context.Autores.AnyAsync(x => x.Nombre.ToLower() == autorDto.Nombre.ToLower());

            if (existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {autorDto.Nombre}");
            }

            var autor = mapper.Map<Autor>(autorDto);

            _context.Add(autor);
            await _context.SaveChangesAsync();

            var autorCreated = mapper.Map<AutorDto>(autor);

            return CreatedAtRoute("ObtenerAutorv1", new { id = autor.Id }, autorCreated);
        }

        [HttpPut("{id}", Name = "actualizarAutorv1")] //Api/autores/1
        public async Task<ActionResult> Put(AutorDto autorDto, int id)
        {

            var exist = await _context.Autores.AnyAsync(x => x.Id == id);

            if (!exist)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorDto);
            autor.Id = id;

            _context.Update(autor);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}", Name = "borrarAutorv1")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await _context.Autores.AnyAsync(x => x.Id == id);

            if (!exist)
            {
                return NotFound();
            }

            _context.Remove(new Autor() { Id = id });
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
