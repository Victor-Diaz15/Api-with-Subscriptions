using AutoMapper;
using WebApiAutores.DTOs;
using WebApiAutores.Entiities;

namespace WebApiAutores.Mappers
{
    public class ComentarioMap : Profile
    {
        public ComentarioMap() 
        {
            CreateMap<ComentarioPostDto, Comentario>()
                .ReverseMap();

            CreateMap<ComentarioDto, Comentario>()
                .ReverseMap();
        }
    }
}
