using DisasterAlert.Models;

namespace DisasterAlert.Services
{
    /// <summary>
    /// Simula a leitura de dados obtidos via satélites GOES-16 (NOAA/NASA) e INPE.
    /// Em produção, esta classe seria substituída por chamadas às APIs reais:
    /// - NASA Earthdata API: https://earthdata.nasa.gov
    /// - INPE Queimadas/Dados Abertos: https://queimadas.dgi.inpe.br
    /// - Copernicus Data Space: https://dataspace.copernicus.eu
    /// Dados simulados com base em padrões climáticos reais das regiões brasileiras.
    /// </summary>
    public class SimulacaoSateliteService
    {
        private readonly Random _random = new Random();

        // Perfis climáticos regionais baseados em dados históricos do INMET/INPE
        private readonly Dictionary<string, (double chuvaBase, double tempBase, double umidBase)> _perfisRegionais = new()
        {
            { "SP", (45, 24, 75) },
            { "RJ", (55, 26, 80) },
            { "SC", (60, 19, 78) },
            { "AM", (90, 30, 88) },
            { "PE", (30, 28, 72) },
            { "RS", (50, 18, 76) },
        };

        public MonitoramentoClimatico GerarLeituraSatelite(Cidade cidade)
        {
            var perfil = _perfisRegionais.TryGetValue(cidade.Estado, out var p)
                ? p : (50, 25, 75);

            // Simula variação climática ao redor dos valores base do perfil regional
            double chuva = Math.Max(0, perfil.chuvaBase + (_random.NextDouble() - 0.3) * 80);
            double temp = perfil.tempBase + (_random.NextDouble() - 0.5) * 12;
            double umidade = Math.Clamp(perfil.umidBase + (_random.NextDouble() - 0.5) * 20, 30, 100);
            double vento = Math.Max(0, 20 + (_random.NextDouble() - 0.2) * 60);

            // 15% de chance de evento extremo (simula detecção de anomalia orbital)
            if (_random.NextDouble() < 0.15)
            {
                chuva *= 2.5;
                vento *= 1.8;
                umidade = Math.Min(umidade * 1.2, 100);
            }

            return new MonitoramentoClimatico
            {
                CidadeId = cidade.Id,
                CidadeNome = cidade.Nome,
                ChuvaAcumuladaMm = Math.Round(chuva, 1),
                TemperaturaC = Math.Round(temp, 1),
                UmidadeRelativa = Math.Round(umidade, 1),
                VelocidadeVentoKmh = Math.Round(vento, 1),
                Fonte = FonteDados.SimuladoGOES16,
                DataHoraRegistro = DateTime.Now
            };
        }

        public List<MonitoramentoClimatico> GerarLeituraEmLote(IEnumerable<Cidade> cidades)
        {
            return cidades.Select(GerarLeituraSatelite).ToList();
        }
    }
}
