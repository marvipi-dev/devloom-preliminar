using DevloomPreliminar.Controllers;
using DevloomPreliminar.Data;
using DevloomPreliminar.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DevloomPreliminar.UnitTest.Controllers;

public class ProdutosControllerUnitTest
{
    [Theory]
    [InlineData(0,0, 10, 10)] // A page é no mínimo 1; size é no mínimo 10
    [InlineData(1,10, 10, 10)] 
    [InlineData(1,10, 15, 10)]
    [InlineData(2,10, 15, 5)]
    [InlineData(1,200, 200, 100)] // Uma página tem no máximo 100 produtos
    public async Task GetAsync_Paginacao_RetornaAQuantidadeEsperadaDeProdutos(int page, int size, int qtdProdutos, int qtdEsperada) // Paginacao
    {
        // Arrange
        var repository = new SqliteDapperRepository();
        
        // Cadastrar a categoria dos produtos a serem gerados
        var categoriasController = new CategoriasController(repository);
        await categoriasController.PostAsync(new() { Nome = "Categoria" });
        var categoriasResult = await categoriasController.GetAsync();
        var categorias = (categoriasResult.Result as OkObjectResult).Value as IEnumerable<CategoriaResponse>;
        var categoriaId = categorias.First().Id;
        
        // Gerar e cadastrar os produtos a serem consultados
        var produtosController = new ProdutosController(repository);
        var produtos = GerarProdutos(qtdProdutos, categoriaId);
        foreach (var produto in produtos)
        {
            await produtosController.PostAsync(produto);
        }
        
        // Act
        var resposta = await produtosController.GetAsync(categoriaId, page, size);
        var produtosRetornados = (resposta.Result as OkObjectResult).Value as IEnumerable<ProdutoResponse>;
        
        // Assert
        Assert.Equal(qtdEsperada, produtosRetornados.Count());
    }

    [Fact]
    public async Task PostAsync_Unicidade_Retorna400SeOProdutoJaPertencerACategoria()
    {
        // Arrange
        var repository = new SqliteDapperRepository();
        var controller = new ProdutosController(repository);
        
        var nomeRepetido = "Repetido";
        var categoriaIdRepetido = Guid.NewGuid();
        var primeiraRequisicao = new ProdutoPostRequest
        {
            Nome = nomeRepetido,
            PrecoUnitario = 0,
            CategoriaId = categoriaIdRepetido
        };
        var segundaRequisicao = new ProdutoPostRequest
        {
            Nome = nomeRepetido,
            PrecoUnitario = 10,
            CategoriaId = categoriaIdRepetido
        };
        
        // Act
        await controller.PostAsync(primeiraRequisicao);
        var resposta = await controller.PostAsync(segundaRequisicao);
        
        // Assert
        Assert.IsType<BadRequestObjectResult>(resposta.Result);
    }
    
    [Fact]
    public async Task PostAsync_PrecoUnitario_Retorna400SeOPrecoUnitarioForNegativo()
    {
        // Arrange
        var repository = new SqliteDapperRepository();
        var controller = new ProdutosController(repository);
        var requisicao = new ProdutoPostRequest
        {
            Nome = "_",
            PrecoUnitario = -1,
            CategoriaId = Guid.NewGuid()
        };
        
        // Act
        var resposta = await controller.PostAsync(requisicao);
        
        // Assert
        Assert.IsType<BadRequestObjectResult>(resposta.Result);
    }
    
    
    private IEnumerable<ProdutoPostRequest> GerarProdutos(int qtdProdutos, Guid categoriaId)
    {
        var produtos = new List<ProdutoPostRequest>();
        
        for (int i = 0; i < qtdProdutos; i++)
        {
            produtos.Add(new()
            {
                Nome = i.ToString(),
                PrecoUnitario = 0,
                CategoriaId = categoriaId
            });
        }

        return produtos;
    }
}