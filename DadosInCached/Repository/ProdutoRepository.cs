using DadosInCached.Interfaces;
using DadosInCached.Interfaces.Base;
using DadosInCached.Models;
using DadosInCached.Repository.Base;

namespace DadosInCached.Repository
{
    public class ProdutoRepository : RepositoryBase<Produto>, IProdutoRepository
    {
        public ProdutoRepository(IServiceProvider service) : base(service)
        {
        }
    }
}
