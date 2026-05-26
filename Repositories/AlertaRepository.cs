using Dapper;
using DisasterAlert.Database;
using DisasterAlert.Models;

namespace DisasterAlert.Repositories
{
    public class AlertaRepository
    {
        public async Task<IEnumerable<AlertaDesastre>> ListarAtivosAsync()
        {
            using var conn = DatabaseConfig.GetConnection();
            var sql = @"
                SELECT a.*, c.Nome AS CidadeNome
                FROM AlertasDesastres a
                INNER JOIN Cidades c ON c.Id = a.CidadeId
                WHERE a.Ativo = 1
                ORDER BY a.IndiceRisco DESC, a.DataHoraAlerta DESC";
            return await conn.QueryAsync<AlertaDesastre>(sql);
        }

        public async Task<IEnumerable<AlertaDesastre>> ListarTodosAsync()
        {
            using var conn = DatabaseConfig.GetConnection();
            var sql = @"
                SELECT a.*, c.Nome AS CidadeNome
                FROM AlertasDesastres a
                INNER JOIN Cidades c ON c.Id = a.CidadeId
                ORDER BY a.DataHoraAlerta DESC";
            return await conn.QueryAsync<AlertaDesastre>(sql);
        }

        public async Task<int> InserirAsync(AlertaDesastre alerta)
        {
            using var conn = DatabaseConfig.GetConnection();
            var sql = @"
                INSERT INTO AlertasDesastres
                    (CidadeId, Nivel, Tipo, IndiceRisco, Descricao, Ativo, DataHoraAlerta)
                VALUES
                    (@CidadeId, @Nivel, @Tipo, @IndiceRisco, @Descricao, @Ativo, @DataHoraAlerta);
                SELECT SCOPE_IDENTITY();";
            return await conn.ExecuteScalarAsync<int>(sql, new
            {
                alerta.CidadeId,
                Nivel = alerta.Nivel.ToString(),
                Tipo = alerta.Tipo.ToString(),
                alerta.IndiceRisco,
                alerta.Descricao,
                alerta.Ativo,
                alerta.DataHoraAlerta
            });
        }

        public async Task EncerrarAlertaAsync(int id)
        {
            using var conn = DatabaseConfig.GetConnection();
            var sql = @"
                UPDATE AlertasDesastres
                SET Ativo = 0, DataHoraEncerramento = GETDATE()
                WHERE Id = @Id";
            await conn.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<IEnumerable<RelatorioResumo>> GerarRelatorioAsync()
        {
            using var conn = DatabaseConfig.GetConnection();
            var sql = @"
                SELECT
                    c.Nome AS CidadeNome,
                    c.Estado,
                    COUNT(m.Id) AS TotalMonitoramentos,
                    ISNULL(AVG(m.ChuvaAcumuladaMm), 0) AS MediaChuva,
                    ISNULL(AVG(m.TemperaturaC), 0) AS MediaTemperatura,
                    ISNULL(AVG(
                        CASE
                            WHEN m.ChuvaAcumuladaMm >= 100 THEN 40
                            WHEN m.ChuvaAcumuladaMm >= 60 THEN 25
                            WHEN m.ChuvaAcumuladaMm >= 30 THEN 10
                            ELSE 0
                        END +
                        CASE
                            WHEN m.UmidadeRelativa >= 90 THEN 20
                            WHEN m.UmidadeRelativa >= 80 THEN 10
                            ELSE 0
                        END
                    ), 0) AS IndiceRiscoMedio,
                    COUNT(CASE WHEN a.Ativo = 1 THEN 1 END) AS AlertasAtivos,
                    ISNULL(MAX(a.Nivel), 'Baixo') AS NivelAlertaAtual,
                    ISNULL(MAX(m.DataHoraRegistro), GETDATE()) AS UltimaAtualizacao
                FROM Cidades c
                LEFT JOIN MonitoramentosClimaticos m ON m.CidadeId = c.Id
                LEFT JOIN AlertasDesastres a ON a.CidadeId = c.Id
                GROUP BY c.Id, c.Nome, c.Estado
                ORDER BY IndiceRiscoMedio DESC";
            return await conn.QueryAsync<RelatorioResumo>(sql);
        }
    }
}
