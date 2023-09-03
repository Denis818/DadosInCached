using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DadosInCached.Context;
using DadosInCached.Models;
using DadosInCached.CustomAttribute;
using DadosInCached.Interfaces;
using AutoMapper;
using DadosInCached.Models.Dtos;

namespace DadosInCached.Controllers
{
    [Cached(15)]
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly IRepositoryBase<Produto> _produtoRepository;
        protected readonly IMapper _mapper;


        public ProdutoController(IRepositoryBase<Produto> produtoRepository, IMapper mapper)
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

            if (produto == null) return NotFound();
            
            return Ok(produto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Produto produto)
        {
            if (id != produto.Id) return BadRequest("Deu Erro!");


            _produtoRepository.UpdateAsync(produto);
            if (!await _produtoRepository.SaveChangesAsync()) return BadRequest("Deu Erro!");

            return Ok(await _produtoRepository.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult<Produto>> Insert(ProdutoDto produtoDto)
        {
           var produto = _mapper.Map<Produto>(produtoDto);

            await _produtoRepository.InsertAsync(produto);
            if (!await _produtoRepository.SaveChangesAsync()) return BadRequest("Deu Erro!");

            return Ok(produto);
        }

        [HttpPost("range")]
        public async Task<IActionResult> PostProdutos(List<ProdutoDto> produtosDto)
        {
            if (produtosDto == null || !produtosDto.Any()) return BadRequest("A lista de produtos está vazia.");
         
            var produtos = _mapper.Map<List<Produto>>(produtosDto);

            _produtoRepository.InsertRange(produtos);
            if (!await _produtoRepository.SaveChangesAsync()) return BadRequest("Deu Erro!");

            return Ok("Produtos adicionados com sucesso!");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);
            if (await _produtoRepository.GetByIdAsync(id) == null) return NotFound();
            
            _produtoRepository.DeleteAsync(produto);
            if (!await _produtoRepository.SaveChangesAsync()) return BadRequest("Deu Erro!");

            return Ok("Deletado!");
        }

        [HttpDelete("range")]
        public async Task<IActionResult> DeleteProdutos([FromBody] List<int> produtoIds)
        {
            if (produtoIds == null || !produtoIds.Any()) return BadRequest("A lista de IDs de produtos está vazia.");
            
            var produtosToRemove = _produtoRepository.Get(p => produtoIds.Contains(p.Id)).ToList();

            if (produtosToRemove.Count == 0) return NotFound("Nenhum produto encontrado com os IDs fornecidos.");
            
            _produtoRepository.DeleteRangeAsync(produtosToRemove);
            if (!await _produtoRepository.SaveChangesAsync()) return BadRequest("Deu Erro!");

            return Ok("Produtos deletados com sucesso!");
        }
    }
}
