using AutoMapper;
using WebApiAutores.DTOs;
using WebApiAutores.Entiities;

namespace WebApiAutores.Mappers
{
    public class LibroMap : Profile
    {
        public LibroMap() 
        {
            CreateMap<Libro, LibroDto>()
                .ReverseMap();

            CreateMap<Libro, LibroConAutoresDto>()
                .ForMember(x => x.Autores, opt => opt.MapFrom(MapLibroDtoAutores))
                .ReverseMap();

            CreateMap<Libro, LibroCreacionDto>()
                .ReverseMap()
                .ForMember(x => x.AutoresLibros, opt => opt.MapFrom(MapAutoresLibros));

            CreateMap<LibroPatchDto, Libro>()
                .ReverseMap();
        }

        private List<AutorDto> MapLibroDtoAutores(Libro libro, LibroDto libroDto)
        {
            var resultado = new List<AutorDto>();

            if (libro.AutoresLibros == null) return resultado;

            foreach(var autorLibro in libro.AutoresLibros)
            {
                resultado.Add(new AutorDto()
                {
                    Id = autorLibro.AutorId,
                    Nombre = autorLibro.Autor.Nombre
                });
            }

            return resultado;
        }

        private List<AutorLibro> MapAutoresLibros(LibroCreacionDto libroCreacionDto, Libro libro)
        {
            List<AutorLibro> autoresLibro = new();

            if (libroCreacionDto.AutoresIds == null) return autoresLibro;
           
            foreach(var autorId in libroCreacionDto.AutoresIds)
            {
                autoresLibro.Add(new AutorLibro { AutorId = autorId });
            }

            return autoresLibro;
        }
    }
}
