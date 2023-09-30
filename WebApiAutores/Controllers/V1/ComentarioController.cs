using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entiities;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/libro/{libroId}/comentarios")]
    public class ComentarioController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<Usuario> _userManager;

        public ComentarioController(ApplicationDbContext context, IMapper mapper,
            UserManager<Usuario> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet("{id}", Name = "obtenerComentarios")]
        public async Task<ActionResult<ComentarioDto>> GetById(int id)
        {
            var comentario = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);

            if (comentario == null) return NotFound($"No se encontro el comentario con el id {id}");

            var comentarioDto = mapper.Map<ComentarioDto>(comentario);

            return Ok(comentarioDto);
        }

        [HttpGet(Name = "obtenerComentario")]
        public async Task<ActionResult<List<ComentarioDto>>> Get(int libroId)
        {
            var existeLibro = await context.Libros.FirstOrDefaultAsync(x => x.Id == libroId);

            if (existeLibro == null)
            {
                return NotFound();
            }

            var comentarios = await context.Comentarios
                .Where(x => x.LibroId == libroId)
                .ToListAsync();

            var comentariosDto = mapper.Map<List<ComentarioDto>>(comentarios);

            return Ok(comentariosDto);
        }

        [HttpPost(Name = "crearComentario")]
        //esto es para proteger la ruta, para acceder a este endpoint debe estar logueado.
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int libroId, ComentarioPostDto dto)
        {
            //obteniendo los claims, que se crearon cuando se genero el token
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "Email").FirstOrDefault();
            var email = emailClaim.Value;

            //Buscando el id logueado por su email
            var usuario = await _userManager.FindByEmailAsync(email);

            var existeLibro = await context.Libros.FirstOrDefaultAsync(x => x.Id == libroId);

            if (existeLibro == null)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(dto);
            comentario.LibroId = libroId;

            //agreando la relacion con el usuario
            comentario.UsuarioId = usuario.Id;
            context.Add(comentario);
            await context.SaveChangesAsync();

            var comentarioCreated = mapper.Map<ComentarioDto>(comentario);

            return CreatedAtRoute("obtenerComentario", new { id = comentario.Id, libroId }, comentarioCreated);
        }

        [HttpPut("{id}", Name = "actualizarComentario")]
        public async Task<ActionResult> Put(int id, int libroId, ComentarioPostDto comentarioPutDto)
        {
            var existLibro = await context.Libros
                .AnyAsync(x => x.Id == libroId);

            if (!existLibro) return NotFound();

            var existComentario = await context.Comentarios
                .AnyAsync(x => x.Id == id);

            if (!existComentario) return NotFound();

            var comentario = mapper.Map<Comentario>(comentarioPutDto);
            comentario.Id = id;
            comentario.LibroId = libroId;

            context.Update(comentario);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
