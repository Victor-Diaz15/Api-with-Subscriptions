using AutoMapper;
using WebApiAutores.DTOs;
using WebApiAutores.Entiities;

namespace WebApiAutores.Mappers
{
    public class LlaveApiMap : Profile
    {
        public LlaveApiMap() 
        {
            CreateMap<LlaveApi, LlaveApiDto>()
                .ReverseMap();
        }
    }
}
