using AutoMapper;
using WebApiAutores.DTOs;
using WebApiAutores.Entiities;

namespace WebApiAutores.Mappers
{
    public class RestriccionDominioMap : Profile
    {
        public RestriccionDominioMap()
        {

            CreateMap<RestriccionDominio, RestriccionDominioDto>()
                .ReverseMap();

            CreateMap<RestriccionDominio, RestriccionDominioCreateDto>()
                .ReverseMap();

            CreateMap<RestriccionDominio, RestriccionDominioUpdateDto>()
                .ReverseMap();
        }
    }
}
