using AutoMapper;
using DadosInCached.Models;
using DadosInCached.Models.Dtos;

namespace Application.Configurations.PerfisAutoMapper
{
    public class EntityMappingProfile : Profile
    {
        public EntityMappingProfile()
        {
            CreateMap<Produto, ProdutoDto>().ReverseMap();
        }
    }
}
