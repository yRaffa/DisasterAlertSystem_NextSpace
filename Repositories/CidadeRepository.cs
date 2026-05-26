using Dapper;
using DisasterAlert.Database;
using DisasterAlert.Models;

namespace DisasterAlert.Repositories
{
    public class CidadeRepository
    {
        public async Task<IEnumerable<Cidade>> ListarTodosAsync()
        {
            using var conn = DatabaseConfig.GetConnection();
            return await conn.QueryAsync<Cidade>("SELECT * FROM Cidades ORDER BY Nome");
        }

        public async Task<Cidade?> BuscarPorIdAsync(int id)
        {
            using var conn = DatabaseConfig.GetConnection();
            return await conn.QueryFirstOrDefaultAsync<Cidade>(
                "SELECT * FROM Cidades WHERE Id = @Id", new { Id = id });
        }

        public async Task<int> InserirAsync(Cidade cidade)
        {
            using var conn = DatabaseConfig.GetConnection();
            var sql = @"
                INSERT INTO Cidades (Nome, Estado, Latitude, Longitude, PopulacaoEstimada, DataCadastro)
                VALUES (@Nome, @Estado, @Latitude, @Longitude, @PopulacaoEstimada, @DataCadastro);
                SELECT SCOPE_IDENTITY();";
            return await conn.ExecuteScalarAsync<int>(sql, cidade);
        }

        public async Task AtualizarAsync(Cidade cidade)
        {
            using var conn = DatabaseConfig.GetConnection();
            var sql = @"
                UPDATE Cidades SET
                    Nome = @Nome,
                    Estado = @Estado,
                    Latitude = @Latitude,
                    Longitude = @Longitude,
                    PopulacaoEstimada = @PopulacaoEstimada
                WHERE Id = @Id";
            await conn.ExecuteAsync(sql, cidade);
        }

        public async Task ExcluirAsync(int id)
        {
            using var conn = DatabaseConfig.GetConnection();
            await conn.ExecuteAsync("DELETE FROM Cidades WHERE Id = @Id", new { Id = id });
        }
    }
}
