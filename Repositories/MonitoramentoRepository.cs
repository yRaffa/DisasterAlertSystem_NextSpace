using Dapper;
using DisasterAlert.Database;
using DisasterAlert.Models;

namespace DisasterAlert.Repositories
{
    public class MonitoramentoRepository
    {
        public async Task<IEnumerable<MonitoramentoClimatico>> ListarPorCidadeAsync(int cidadeId)
        {
            using var conn = DatabaseConfig.GetConnection();
            var sql = @"
                SELECT m.*, c.Nome AS CidadeNome
                FROM MonitoramentosClimaticos m
                INNER JOIN Cidades c ON c.Id = m.CidadeId
                WHERE m.CidadeId = @CidadeId
                ORDER BY m.DataHoraRegistro DESC";
            return await conn.QueryAsync<MonitoramentoClimatico>(sql, new { CidadeId = cidadeId });
        }

        public async Task<IEnumerable<MonitoramentoClimatico>> ListarTodosRecentesAsync(int top = 50)
        {
            using var conn = DatabaseConfig.GetConnection();
            var sql = $@"
                SELECT TOP {top} m.*, c.Nome AS CidadeNome
                FROM MonitoramentosClimaticos m
                INNER JOIN Cidades c ON c.Id = m.CidadeId
                ORDER BY m.DataHoraRegistro DESC";
            return await conn.QueryAsync<MonitoramentoClimatico>(sql);
        }

        public async Task<int> InserirAsync(MonitoramentoClimatico m)
        {
            using var conn = DatabaseConfig.GetConnection();
            var sql = @"
                INSERT INTO MonitoramentosClimaticos
                    (CidadeId, ChuvaAcumuladaMm, TemperaturaC, UmidadeRelativa, VelocidadeVentoKmh, Fonte, DataHoraRegistro)
                VALUES
                    (@CidadeId, @ChuvaAcumuladaMm, @TemperaturaC, @UmidadeRelativa, @VelocidadeVentoKmh, @Fonte, @DataHoraRegistro);
                SELECT SCOPE_IDENTITY();";
            return await conn.ExecuteScalarAsync<int>(sql, new
            {
                m.CidadeId,
                m.ChuvaAcumuladaMm,
                m.TemperaturaC,
                m.UmidadeRelativa,
                m.VelocidadeVentoKmh,
                Fonte = m.Fonte.ToString(),
                m.DataHoraRegistro
            });
        }

        public async Task ExcluirAsync(int id)
        {
            using var conn = DatabaseConfig.GetConnection();
            await conn.ExecuteAsync("DELETE FROM MonitoramentosClimaticos WHERE Id = @Id", new { Id = id });
        }
    }
}
