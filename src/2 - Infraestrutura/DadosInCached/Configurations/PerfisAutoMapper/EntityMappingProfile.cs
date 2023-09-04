using AutoMapper;
using Domain.Dtos;
using Domain.Models;

namespace DadosInCached.Configurations.PerfisAutoMapper
{
    public class EntityMappingProfile : Profile
    {
        public EntityMappingProfile()
        {
            CreateMap<Produto, ProdutoDto>().ReverseMap();
        }
    }
}
