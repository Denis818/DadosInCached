using Data.Repository.Base;
using Domain.Models;
using Domain.Interfaces.Repository;

namespace Data.Repository
{
    public class ProdutoRepository : RepositoryBase<Produto>, IProdutoRepository
    {
        public ProdutoRepository(IServiceProvider service) : base(service)
        {
        }
    }
}
