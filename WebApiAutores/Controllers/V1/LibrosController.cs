using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entiities;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly IMapper mapper;

        public ApplicationDbContext _context { get; }
        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id}", Name = "obtenerLibro")]
        public async Task<ActionResult<LibroConAutoresDto>> Get(int id)
        {
            var libro = await _context.Libros
                .Include(x => x.AutoresLibros)
                .ThenInclude(x => x.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();

            return Ok(mapper.Map<LibroConAutoresDto>(libro));
        }

        [HttpPost(Name = "crearLibro")]
        public async Task<ActionResult<Libro>> Post(LibroCreacionDto libroCreacionDto)
        {
            if (libroCreacionDto.AutoresIds == null) return BadRequest("No se puede crear un libro sin autores");

            var autoresIds = await _context.Autores.Where(x => libroCreacionDto.AutoresIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            if (autoresIds.Count != libroCreacionDto.AutoresIds.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }

            var libro = mapper.Map<Libro>(libroCreacionDto);

            AsignarOrden(libro);

            _context.Add(libro);
            await _context.SaveChangesAsync();

            var libroCreated = mapper.Map<LibroDto>(libro);

            return CreatedAtRoute("ObtenerLibro", new { id = libro.Id }, libroCreated);
        }

        [HttpPut("{id}", Name = "actualizarLibro")]
        public async Task<ActionResult> Put(int id, LibroCreacionDto libroPutDto)
        {
            var libroDb = await _context.Libros
                .Include(x => x.AutoresLibros)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libroDb == null) return NotFound();

            libroDb = mapper.Map(libroPutDto, libroDb);
            AsignarOrden(libroDb);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "patchLibro")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDto> patchDocument)
        {
            if (patchDocument == null) return BadRequest();

            var libroDb = await _context.Libros
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libroDb == null) return NotFound();

            var libroDto = mapper.Map<LibroPatchDto>(libroDb);

            patchDocument.ApplyTo(libroDto, ModelState);

            var isValid = TryValidateModel(libroDto);

            if (!isValid) return BadRequest(ModelState);

            mapper.Map(libroDto, libroDb);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}", Name = "borrarLibro")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await _context.Libros.AnyAsync(x => x.Id == id);

            if (!exist)
            {
                return NotFound();
            }

            _context.Remove(new Libro() { Id = id });
            await _context.SaveChangesAsync();
            return Ok();
        }

        //Metodos privados
        private void AsignarOrden(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }

    }
}
