using DevloomPreliminar.Data;
using DevloomPreliminar.ExtensionMethods;
using DevloomPreliminar.Models;
using DevloomPreliminar.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace DevloomPreliminar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoRepository _repository;

        public ProdutosController(IProdutoRepository repository)
        {
            _repository = repository;
        }
        
        // GET api/<ProdutosController>/5
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProdutoResponse>>> GetAsync([FromQuery] Guid categoriaId, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            page = page < 1 
                ? 1 
                : page;
            
            size = size < 10 ? 10 
                : size > 100 ? 100
                : size;
            
            var produtos = await _repository.LerProdutosAsync(categoriaId, page, size);
            
            var resposta = produtos.Select(p => new ProdutoResponse()
            {
                Id = p.Id,
                Nome = p.Nome,
                PrecoUnitario = p.PrecoUnitario,
                CategoriaId = p.CategoriaId
            });
            
            return Ok(resposta);
        }

        // POST api/<ProdutosController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ProdutoResponse>> PostAsync(ProdutoPostRequest produto)
        {
            var produtoCriar = new ProdutoModel()
            {
                Id = Guid.NewGuid(),
                Nome = produto.Nome!,
                PrecoUnitario = produto.PrecoUnitario,
                CategoriaId = produto.CategoriaId
            };

            try
            {
                await _repository.CriarAsync(produtoCriar);
            }
            catch (SqliteException e)
            {
                var erros = FormatarMsgErro(produto, e);
                var problemas = erros.ToProblemDetails(HttpContext);
                return BadRequest(problemas);
            }
            
            return Created();
        }

        private static Dictionary<string, string[]> FormatarMsgErro(ProdutoPostRequest produto, SqliteException excecao)
        {
            var erros = new Dictionary<string, string[]>();

            switch (excecao.Message)
            {
                case "SQLite Error 19: 'FOREIGN KEY constraint failed'.":
                    erros.Add("Produto.CategoriaId",
                        [$"A categoria '{produto.CategoriaId}' não está cadastrada no sistema."]);
                    break;
                    
                case "SQLite Error 19: 'UNIQUE constraint failed: Produto.Nome, Produto.CategoriaId'.":
                    var msgErro = new[] {$"Já existe um produto '{produto.Nome}' vinculado à categoria '{produto.CategoriaId}'."};
                    erros.Add("Produto.Nome", msgErro);
                    erros.Add("Produto.CategoriaId", msgErro);
                    break;
            }

            return erros;
        }
    }
}
