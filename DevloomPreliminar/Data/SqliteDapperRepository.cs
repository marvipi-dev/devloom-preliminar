using Dapper;
using DevloomPreliminar.Models;
using Microsoft.Data.Sqlite;

namespace DevloomPreliminar.Data;

public class SqliteDapperRepository : ICategoriaRepository, IProdutoRepository
{
    private static readonly SqliteConnection Connection;

    static SqliteDapperRepository()
    {
        Connection = new SqliteConnection("Data Source=:memory:");
        Connection.Open();
    }

    public SqliteDapperRepository()
    {
        var inicializarTabelasCmd = new CommandDefinition("""
                                                          CREATE TABLE IF NOT EXISTS Categoria
                                                          (
                                                              Id TXT PRIMARY KEY NOT NULL,
                                                              Nome TXT UNIQUE NOT NULL CHECK(Length(Nome) <= 50)
                                                          );

                                                          CREATE TABLE IF NOT EXISTS Produto
                                                          (
                                                              Id TXT NOT NULL,
                                                              Nome TXT NOT NULL CHECK(Length(Nome) <= 100),
                                                              PrecoUnitario REAL NOT NULL CHECK(PrecoUnitario >= 0),
                                                              CategoriaId TXT NOT NULL,
                                                              PRIMARY KEY (Id, CategoriaId),
                                                              UNIQUE (Nome, CategoriaId),
                                                              FOREIGN KEY(CategoriaId) REFERENCES Categoria(Id)
                                                          );
                                                          """);

        Connection.Execute(inicializarTabelasCmd);
    }
    
    public async Task<IEnumerable<CategoriaModel>> LerCategoriasAsync()
    {
        var selectCmd = new CommandDefinition(
            """
                        SELECT * FROM Categoria;
                        """);
        
        var categorias = await Connection.QueryAsync(selectCmd);
        
        return categorias.Select(d => new CategoriaModel()
        {
            Id = Guid.Parse(d.Id),
            Nome = d.Nome
        });
    }

    public async Task<CategoriaModel?> CriarAsync(CategoriaModel categoria)
    {
        var insertCmd = new CommandDefinition(
            """
            INSERT INTO Categoria (Id, Nome) VALUES (@Id, @Nome)
            """, categoria);
        
        var linhasAlteradas = await Connection.ExecuteAsync(insertCmd);
        
        return linhasAlteradas == 1
            ? categoria
            : null;
    }

    public async Task<IEnumerable<ProdutoModel>> LerProdutosAsync(Guid categoriaId, int page, int size)
    {
        var selectCmd = new CommandDefinition(
        """
                    SELECT * FROM Produto
                    WHERE CategoriaId == @categoriaId
                    LIMIT @size OFFSET (@page - 1) * @size;
                    """, new { categoriaId, page, size });
        
        var produtos = await Connection.QueryAsync(selectCmd);
        
        return produtos.Select(d => new ProdutoModel()
        {
            Id = Guid.Parse(d.Id),
            Nome = d.Nome.ToString(), // Causa exceção quando ele precisa ser convertido implicitamente para string
            PrecoUnitario = (decimal) d.PrecoUnitario,
            CategoriaId = Guid.Parse(d.CategoriaId)
        });
    }

    public async Task<ProdutoModel?> CriarAsync(ProdutoModel produto)
    {
        var insertCmd = new CommandDefinition("""
                                              INSERT INTO Produto (Id, Nome, PrecoUnitario, CategoriaId)
                                              VALUES (@Id, @Nome, @PrecoUnitario, @CategoriaId)
                                              """, produto);

        var linhasAlteradas = await Connection.ExecuteAsync(insertCmd);

        return linhasAlteradas == 1
            ? produto
            : null;
    }
}