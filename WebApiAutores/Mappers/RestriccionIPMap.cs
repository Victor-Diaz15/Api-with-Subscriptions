using AutoMapper;
using WebApiAutores.DTOs;
using WebApiAutores.Entiities;

namespace WebApiAutores.Mappers
{
    public class RestriccionIPMap : Profile
    {
        public RestriccionIPMap()
        {
            CreateMap<RestriccionIP, RestriccionIPCreateDto>()
                .ReverseMap();

            CreateMap<RestriccionIP, RestriccionDominioUpdateDto>()
                .ReverseMap();

            CreateMap<RestriccionIP, RestriccionIPDto>()
                .ReverseMap();
        }
    }
}
