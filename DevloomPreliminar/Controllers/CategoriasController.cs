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
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepository _repository;

        public CategoriasController(ICategoriaRepository repository)
        {
            _repository = repository;
        }
        
        // GET: api/<CategoriasController>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CategoriaResponse>>> GetAsync()
        {
            var categorias = await _repository.LerCategoriasAsync();
            
            var resposta = categorias.Select(c => new CategoriaResponse()
            {
                Id = c.Id,
                Nome = c.Nome
            });
            
            return Ok(resposta);
        }

        // POST api/<CategoriasController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<CategoriaResponse>> PostAsync(CategoriaPostRequest categoria)
        {
            var categoriaCriar = new CategoriaModel()
            {
                Id = Guid.NewGuid(),
                Nome = categoria.Nome!
            };
            
            try
            {
                await _repository.CriarAsync(categoriaCriar);
            }
            catch (SqliteException e)
            {
                var erros = FormatarMsgErro(categoria, e);
                var problemas = erros.ToProblemDetails(HttpContext);
                return BadRequest(problemas);
            }
            
            return Created();
        }

        private static Dictionary<string, string[]> FormatarMsgErro(CategoriaPostRequest categoria, SqliteException excecao)
        {
            var erros = new Dictionary<string, string[]>();

            switch (excecao.Message){
                case "SQLite Error 19: 'UNIQUE constraint failed: Categoria.Nome'.":
                    erros.Add("Categoria.Nome",
                        [$"A categoria '{categoria.Nome}' já está cadastrada no sistema."]);
                    break;
            }

            return erros;
        }
    }
}
