using Microsoft.Data.SqlClient;

namespace DisasterAlert.Database
{
    public class DatabaseConfig
    {
        // Altere a connection string conforme sua configuração do SQL Server no DBeaver
        private const string ConnectionString =
            "Server=localhost\\SQLEXPRESS;Database=DisasterAlertDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public static async Task InicializarBancoDeDadosAsync()
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            var sql = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Cidades' AND xtype='U')
                CREATE TABLE Cidades (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Nome NVARCHAR(100) NOT NULL,
                    Estado NVARCHAR(50) NOT NULL,
                    Latitude FLOAT NOT NULL,
                    Longitude FLOAT NOT NULL,
                    PopulacaoEstimada FLOAT NOT NULL,
                    DataCadastro DATETIME NOT NULL DEFAULT GETDATE()
                );

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='MonitoramentosClimaticos' AND xtype='U')
                CREATE TABLE MonitoramentosClimaticos (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    CidadeId INT NOT NULL REFERENCES Cidades(Id),
                    ChuvaAcumuladaMm FLOAT NOT NULL,
                    TemperaturaC FLOAT NOT NULL,
                    UmidadeRelativa FLOAT NOT NULL,
                    VelocidadeVentoKmh FLOAT NOT NULL,
                    Fonte NVARCHAR(50) NOT NULL,
                    DataHoraRegistro DATETIME NOT NULL DEFAULT GETDATE()
                );

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AlertasDesastres' AND xtype='U')
                CREATE TABLE AlertasDesastres (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    CidadeId INT NOT NULL REFERENCES Cidades(Id),
                    Nivel NVARCHAR(20) NOT NULL,
                    Tipo NVARCHAR(30) NOT NULL,
                    IndiceRisco FLOAT NOT NULL,
                    Descricao NVARCHAR(500) NOT NULL,
                    Ativo BIT NOT NULL DEFAULT 1,
                    DataHoraAlerta DATETIME NOT NULL DEFAULT GETDATE(),
                    DataHoraEncerramento DATETIME NULL
                );
            ";

            using var cmd = new SqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task SeedDadosIniciaisAsync()
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            // Verifica se já há dados
            using var checkCmd = new SqlCommand("SELECT COUNT(*) FROM Cidades", conn);
            int count = (int)await checkCmd.ExecuteScalarAsync()!;
            if (count > 0) return;

            var seedSql = @"
                INSERT INTO Cidades (Nome, Estado, Latitude, Longitude, PopulacaoEstimada) VALUES
                ('São Paulo', 'SP', -23.5505, -46.6333, 12300000),
                ('Petrópolis', 'RJ', -22.5050, -43.1789, 306700),
                ('Blumenau', 'SC', -26.9194, -49.0661, 352200),
                ('Manaus', 'AM', -3.1190, -60.0217, 2219580),
                ('Recife', 'PE', -8.0522, -34.9286, 1661017),
                ('Porto Alegre', 'RS', -30.0346, -51.2177, 1492530);
            ";

            using var seedCmd = new SqlCommand(seedSql, conn);
            await seedCmd.ExecuteNonQueryAsync();
        }
    }
}
