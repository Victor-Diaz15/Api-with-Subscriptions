using AutoMapper;
using WebApiAutores.DTOs;
using WebApiAutores.Entiities;

namespace WebApiAutores.Mappers
{
    public class AutorMap : Profile
    {
        public AutorMap() 
        {
            CreateMap<Autor, AutorDto>()
                .ReverseMap();

            CreateMap<Autor, AutorConLibrosDto>()
                .ForMember(x => x.Libros, op => op.MapFrom(MapAutorDtoLibros))
                .ReverseMap();
        }

        private List<LibroDto> MapAutorDtoLibros(Autor autor, AutorDto autorDto)
        {
            var resultado = new List<LibroDto>();

            if (autor.AutoresLibros == null) return resultado;

            foreach (var autorLibro in autor.AutoresLibros)
            {
                resultado.Add(new LibroDto()
                {
                    Id = autorLibro.LibroId,
                    Titulo = autorLibro.Libro.Titulo
                });
            }

            return resultado;
        }
    }
}
