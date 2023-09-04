using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Domain.Interfaces.Repository;
using DadosInCached.Attributes;
using Domain.Models;

namespace DadosInCached.Controllers
{
    [Cached(15)]
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoRepository _produtoRepository;
        protected readonly IMapper _mapper;

        public ProdutoController(IProdutoRepository produtoRepository, IMapper mapper)
        {
            _produtoRepository = produtoRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetAll()
        {
            var list = await _produtoRepository.Get().ToListAsync();
            var resposne = new
            {
                Registros = list.Count,
                Dados = list
            };
            return Ok(resposne);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetById(int id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);

            if (produto == null) return NotFound("Não encontrado.");
            
            return Ok(produto);
        }
    }
}
